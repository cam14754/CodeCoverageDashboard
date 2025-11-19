from __future__ import annotations
import os
import time
import json
import sys
import sqlite3
from dataclasses import dataclass, field
from datetime import datetime, timedelta
from typing import List, Optional, Any
from xmlrpc.client import Boolean

from xsdata.formats.dataclass.parsers import XmlParser

# ==============================================
# Models:
# ==============================================

@dataclass
class RepoData:
    name: str = "Unknown Name"
    list_errors: List[str] = field(default_factory=list)

    coverage_percent: float = 0.0
    coverage_percent_increase: float = 0.0

    covered_lines: float = 0.0
    covered_lines_increase: float = 0.0

    total_lines: float = 0.0

    uncovered_lines: float = 0.0
    total_branches: float = 0.0
    total_covered_branches: float = 0.0
    branch_rate: float = 0.0

    date_retrieved: datetime = field(default_factory=datetime.now)

    list_classes: List["ClassData"] = field(default_factory=list)

    def __str__(self):
        return f"RepoData(Name: {self.name}, Time: {self.date_retrieved}, Coverage Percent: {self.coverage_percent})"

    def to_dict(self):
        return {
            "Name": self.name,
            "CoveragePercent": self.coverage_percent,
            "CoveragePercentPercentIncrease": self.coverage_percent_increase,
            "CoveredLines": self.covered_lines,
            "CoveredLinesIncrease": self.covered_lines_increase,
            "TotalLines": self.total_lines,
            "UncoveredLines": self.uncovered_lines,
            "TotalBranches": self.total_branches,
            "TotalCoveredBranches": self.total_covered_branches,
            "BranchRate": self.branch_rate,
            "DateRetrieved": ensure_datetime(self.date_retrieved).isoformat(),
            "ListClasses": [c.to_dict() for c in self.list_classes]
        }


@dataclass
class ClassData:
    name: str = "Unknown Name"
    file_path: str = "Unknown File Path"

    coverage_percent: float = 0.0
    branch_coverage_percent: float = 0.0
    total_lines: float = 0.0
    covered_lines: float = 0.0

    date_retrieved: datetime = datetime.fromtimestamp(0)  # UnixEpoch

    list_methods: List["MethodData"] = field(default_factory=list)

    def to_dict(self):
        return {
            "Name": self.name,
            "FilePath": self.file_path,
            "CoveragePercent": self.coverage_percent,
            "BranchCoveragePercent": self.branch_coverage_percent,
            "TotalLines": self.total_lines,
            "CoveredLines": self.covered_lines,
            "DateRetrieved": ensure_datetime(self.date_retrieved).isoformat(),
            "ListMethods": [m.to_dict() for m in self.list_methods]
        }


@dataclass
class MethodData:
    name: Optional[str] = "Unknown Name"
    errors: List[str] = field(default_factory=list)

    coverage_percent: float = 0.0
    branch_coverage_percent: float = 0.0

    complexity: float = 0.0
    list_lines: List["LineData"] = field(default_factory=list)

    signature: Optional[str] = "()"

    parent_repo: Optional[str] = "Unknown Parent Repo"

    def to_dict(self):
        return {
            "Name": self.name,
            "Signature": self.signature,
            "CoveragePercent": self.coverage_percent,
            "BranchCoveragePercent": self.branch_coverage_percent,
            "Complexity": self.complexity,
            "ListLines": [l.to_dict() for l in self.list_lines],
            "ParentRepoName": self.parent_repo
        }


@dataclass
class LineData:
    line_number: Optional[float] = None
    hits: Optional[float] = None

    def to_dict(self):
        return {
            "LineNumber": self.line_number,
            "Hits": self.hits
        }

@dataclass
class StaticDashboardData:
    # Calculated Properties (stored)
    total_lines_covered_count: float = 0.0
    total_lines_uncovered_count: float = 0.0
    total_repos_count: float = 0.0
    total_classes_count: float = 0.0
    total_methods_count: float = 0.0
    total_lines_count: float = 0.0

    average_branch_coverage_percent: float = 0.0
    average_line_coverage_percent: float = 0.0
    total_branches_covered_count: float = 0.0
    complex_method_count: float = 0.0
    average_complex_method_percent: float = 0.0

    # Lists
    list_repos: List["RepoData"] = field(default_factory=list)

    list_hot_repos: List["RepoData"] = field(default_factory=list)
    list_healthy_repos: List["RepoData"] = field(default_factory=list)
    list_unhealthy_repos: List["RepoData"] = field(default_factory=list)
    list_complex_methods: List["MethodData"] = field(default_factory=list)

    # Given Properties
    date_retrieved: datetime = field(default_factory=lambda: datetime.fromtimestamp(0))
    data_age: datetime = field(default_factory=lambda: datetime.fromtimestamp(0))
    coverlet_version: str = ""
    dashboard_version: str = ""

    def __str__(self):
        return f"StaticDashboardData(Total Lines: {self.total_lines_count}, Time: {self.date_retrieved}, Average Coverage Percent: {self.average_line_coverage_percent})"

# ==============================================
# DTOs
# ==============================================

@dataclass
class SourceDTO:
    class Meta:
        name = "source"

    path: str = field(
        default="",
        metadata={
            "type": "Text",
        },
    )


@dataclass
class ConditionDTO:
    class Meta:
        name = "condition"

    number: int = field(
        default=0,
        metadata={
            "type": "Attribute",
            "name": "number",
        },
    )

    type: str = field(
        default="",
        metadata={
            "type": "Attribute",
            "name": "type",
        },
    )

    coverage: str = field(
        default="",
        metadata={
            "type": "Attribute",
            "name": "coverage",
        },
    )


@dataclass
class LineDTO:
    class Meta:
        name = "line"

    number: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "number",
        },
    )

    hits: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "hits",
        },
    )

    branch: str = field(
        default="false",
        metadata={
            "type": "Attribute",
            "name": "branch",
        },
    )

    condition_coverage: str = field(
        default="",
        metadata={
            "type": "Attribute",
            "name": "condition-coverage",
        },
    )

    # <conditions><condition .../></conditions>
    conditions: List[ConditionDTO] = field(
        default_factory=list,
        metadata={
            "type": "Element",
            "name": "condition",
            "wrapper": "conditions",
        },
    )


@dataclass
class MethodDTO:
    class Meta:
        name = "method"

    name: str = field(
        default="",
        metadata={
            "type": "Attribute",
            "name": "name",
        },
    )

    line_rate: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "line-rate",
        },
    )

    signature: str = field(
        default="()",
        metadata={
            "type": "Attribute",
            "name": "signature",
        },
    )

    complexity: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "complexity",
        },
    )

    branch_rate: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "branch-rate",
        },
    )

    # <lines><line .../></lines>
    lines: List[LineDTO] = field(
        default_factory=list,
        metadata={
            "type": "Element",
            "name": "line",
            "wrapper": "lines",
        },
    )


@dataclass
class ClassDTO:
    class Meta:
        name = "class"

    name: str = field(
        default="",
        metadata={
            "type": "Attribute",
            "name": "name",
        },
    )

    file_name: str = field(
        default="",
        metadata={
            "type": "Attribute",
            "name": "filename",
        },
    )

    line_rate: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "line-rate",
        },
    )

    branch_rate: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "branch-rate",
        },
    )

    complexity: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "complexity",
        },
    )

    methods: List[MethodDTO] = field(
        default_factory=list,
        metadata={
            "type": "Element",
            "name": "method",
            "wrapper": "methods",
        },
    )

    lines: List[LineDTO] = field(
        default_factory=list,
        metadata={
            "type": "Element",
            "name": "line",
            "wrapper": "lines",
        },
    )


@dataclass
class PackageDTO:
    class Meta:
        name = "package"

    name: str = field(
        default="",
        metadata={
            "type": "Attribute",
            "name": "name",
        },
    )

    line_rate: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "line-rate",
        },
    )

    branch_rate: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "branch-rate",
        },
    )

    complexity: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "complexity",
        },
    )

    classes: List[ClassDTO] = field(
        default_factory=list,
        metadata={
            "type": "Element",
            "name": "class",
            "wrapper": "classes",
        },
    )


@dataclass
class CoverageDTO:
    class Meta:
        name = "coverage"

    line_rate: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "line-rate",
        },
    )

    branch_rate: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "branch-rate",
        },
    )

    version: str = field(
        default="",
        metadata={
            "type": "Attribute",
            "name": "version",
        },
    )

    timestamp: int = field(
        default=0,
        metadata={
            "type": "Attribute",
            "name": "timestamp",
        },
    )

    lines_covered: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "lines-covered",
        },
    )

    lines_valid: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "lines-valid",
        },
    )

    branches_covered: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "branches-covered",
        },
    )

    branches_valid: float = field(
        default=0.0,
        metadata={
            "type": "Attribute",
            "name": "branches-valid",
        },
    )

    sources: List[SourceDTO] = field(
        default_factory=list,
        metadata={
            "type": "Element",
            "name": "source",
            "wrapper": "sources",
        },
    )

    packages: List[PackageDTO] = field(
        default_factory=list,
        metadata={
            "type": "Element",
            "name": "package",
            "wrapper": "packages",
        },
    )


@dataclass
class DashboardRecordPy:
    id: int | None
    date_retrieved: datetime
    properties_text: str

    @property
    def properties(self) -> dict[str, Any]:
        if not self.properties_text:
            return {}
        return json.loads(self.properties_text)

    @properties.setter
    def properties(self, value: dict[str, Any]) -> None:
        self.properties_text = json.dumps(value)


# ==============================================
# Functions
# ==============================================

DB_LOCATION = r"\\redstorage4.esri.com\AppsBuild\CodeCoverageDashboard\CodeCoverageDataBase.db"
XML_LOCATION = r"C:\Users\cam14754\Desktop\Reports"
DOTNET_TICKS_PER_SECOND = 10_000_000
DOTNET_EPOCH = datetime(1, 1, 1)
UPLOAD_TO_DB_BOOL = Boolean
DASHBOARD_VERSION = "0.4.1"

def Execute():
    latest_full_path = GetLatestReportDir()
    if latest_full_path is None:
        return

    list_coverages = ParseXML(latest_full_path)

    list_repo_datas = AnalyzeCoverageDTO(list_coverages)

    dashboard_data = CreateStaticDashboardDataModel(list_repo_datas)

    if UPLOAD_TO_DB_BOOL:
        print("Uploading to DB")
        upload_dashboard_to_database(dashboard_data)
    else:
        print("Choose not to upload to DB")

def GetLatestReportDir():
    repo_dir_list = os.listdir(XML_LOCATION)

    list_of_repo_data = []

    for repo_dir in repo_dir_list:
        splits = repo_dir.split("_")

        if len(splits) < 3:
            print(f"Skipping invalid file name: {repo_dir}")
            continue

        try:
            date_obj = datetime.strptime(splits[0], "%Y-%m-%d")
        except ValueError:
            print(f"Invalid date format in: {repo_dir}")
            continue

        time_str = splits[1]
        key_str = splits[2]

        report_tuple = (date_obj, time_str, key_str, repo_dir)
        list_of_repo_data.append(report_tuple)

    if not list_of_repo_data:
        print("No valid reports found.")
        return None

    latest_report = max(list_of_repo_data, key=lambda t: t[0])

    latest_date, latest_time, latest_key, latest_dirname = latest_report

    print("Latest report directory:")
    print("  Dir name:", latest_dirname)
    print("  Date    :", latest_date.strftime("%Y-%m-%d"))
    print("  Time    :", latest_time)
    print("  Key     :", latest_key)

    latest_full_path = os.path.join(XML_LOCATION, latest_dirname)

    return latest_full_path

def ParseXML(path_to_latest_dir) -> List[CoverageDTO]:
    parser = XmlParser()
    coverage_list: List[CoverageDTO] = []

    for file_name in os.listdir(path_to_latest_dir):
        if not file_name.lower().endswith(".xml"):
            continue

        full_path = os.path.join(path_to_latest_dir, file_name)

        with open(full_path, "r", encoding="utf-8") as f:
            xml_string = f.read()

        coverage: CoverageDTO = parser.from_string(xml_string, CoverageDTO)
        coverage_list.append(coverage)

    return coverage_list

def AnalyzeCoverageDTO(coverage_list: List["CoverageDTO"]) -> List["RepoData"]:
    repodata: List["RepoData"] = []

    for coverage in coverage_list:
        data = RepoData()

        package: PackageDTO = coverage.packages[0]

        #Meta Data
        data.name = package.name
        data.date_retrieved = ensure_datetime(coverage.timestamp)


        #Root Attributes
        data.covered_lines = coverage.lines_covered
        data.total_lines = coverage.lines_valid
        data.coverage_percent = coverage.line_rate
        data.branch_rate = coverage.branch_rate
        data.total_branches = coverage.branches_valid
        data.total_covered_branches = coverage.branches_covered
        data.uncovered_lines = data.total_lines - data.covered_lines

        list_classes: list[ClassData] = []

        for c in package.classes:
            list_methods: list[MethodData] = []

            for m in c.methods:
                # Build list of LineData
                lines = [
                    LineData(line_number=l.number, hits=l.hits)
                    for l in m.lines
                ]

                # Build MethodData
                list_methods.append(
                    MethodData(
                        name=m.name,
                        coverage_percent=m.line_rate,
                        list_lines=lines,
                        signature=m.signature,
                        complexity=m.complexity,
                        branch_coverage_percent=m.branch_rate,
                        parent_repo=data.name
                    )
                )

            # Compute TotalLines and CoveredLines
            total_lines = sum(len(m.lines) for m in c.methods)
            covered_lines = sum(
                1
                for m in c.methods
                for l in m.lines
                if l.hits > 0
            )

            # Build ClassData
            list_classes.append(
                ClassData(
                    name=c.name,
                    file_path=c.file_name,
                    coverage_percent=c.line_rate,
                    branch_coverage_percent=c.branch_rate,
                    total_lines=total_lines,
                    covered_lines=covered_lines,
                    date_retrieved=data.date_retrieved,
                    list_methods=list_methods,
                )
            )

        # Order classes by name
        list_classes = sorted(list_classes, key=lambda lc: lc.name)

        # Assign back to repo_data
        data.list_classes = list_classes

        repodata.append(data)

    return repodata

def CreateStaticDashboardDataModel(repo_datas: List["RepoData"]) -> StaticDashboardData:
    dashboard_data = StaticDashboardData()
    dashboard_data.total_repos_count = len(repo_datas)
    dashboard_data.date_retrieved = datetime.now()

    # dynamically allocated
    dashboard_data.coverlet_version = "6.0.2"
    dashboard_data.dashboard_version = DASHBOARD_VERSION

    if repo_datas:
        dashboard_data.data_age = repo_datas[0].date_retrieved

    average_coverage_percent_sum = 0.0
    average_branch_coverage_percent_sum = 0.0

    #Calculate big counts
    for repo in repo_datas:
        dashboard_data.list_repos.append(repo)

        average_coverage_percent_sum += repo.coverage_percent
        average_branch_coverage_percent_sum += repo.branch_rate
        dashboard_data.total_branches_covered_count += repo.total_covered_branches

        for class_data in repo.list_classes:
            dashboard_data.total_classes_count += 1

            for method_data in class_data.list_methods:
                dashboard_data.total_methods_count += 1

                for line_data in method_data.list_lines:
                    dashboard_data.total_lines_count += 1
                    if line_data.hits >= 1:
                        dashboard_data.total_lines_covered_count += 1
                    else:
                        dashboard_data.total_lines_uncovered_count += 1

    # Calculate Averages
    if repo_datas:
        repo_count = len(repo_datas)
        dashboard_data.average_line_coverage_percent = (
                average_coverage_percent_sum / repo_count
        )
        dashboard_data.average_branch_coverage_percent = (
                average_branch_coverage_percent_sum / repo_count
        )

    # Get Previous Data
    week_old_data = LoadWeekOldData()

    previous_by_name = {
        r.name: r
        for r in week_old_data.list_repos
        if r is not None and r.name and r.name.strip()
    }

    for latest_repo in dashboard_data.list_repos:
        if latest_repo is None or not latest_repo.name:
            print("null repo?")
            continue

        match = previous_by_name.get(latest_repo.name)

        if match is None:
            # No previous repo with this name: increase == current
            latest_repo.covered_lines_increase = latest_repo.covered_lines
            latest_repo.coverage_percent_increase = latest_repo.coverage_percent
        else:
            # Compute deltas
            latest_repo.covered_lines_increase = (
                    latest_repo.covered_lines - match.covered_lines
            )
            latest_repo.coverage_percent_increase = (
                    latest_repo.coverage_percent - match.coverage_percent
            )

    # Calculate complex methods
    all_complex_methods = [
        method
        for repo in dashboard_data.list_repos
        for cls in repo.list_classes
        for method in cls.list_methods
        if method.complexity >= 10
    ]

    dashboard_data.complex_method_count = len(all_complex_methods)
    dashboard_data.average_complex_method_percent = dashboard_data.complex_method_count / dashboard_data.total_methods_count

    # Top 5 most complex methods
    dashboard_data.list_complex_methods = sorted(
        all_complex_methods,
        key=lambda m: m.complexity,
        reverse=True
    )[:5]

    dashboard_data.list_healthy_repos = sorted(
        dashboard_data.list_repos,
        key=lambda r: r.coverage_percent,
        reverse=True
    )[:5]

    dashboard_data.list_unhealthy_repos = sorted(
        dashboard_data.list_repos,
        key=lambda r: r.coverage_percent
    )[:5]

    dashboard_data.list_hot_repos = sorted(
        dashboard_data.list_repos,
        key=lambda r: r.covered_lines_increase,
        reverse=True
    )[:5]

    return dashboard_data

def LoadWeekOldData() -> StaticDashboardData:
    """Return the Dashboard closest to exactly 7 days ago (by hour),
    but only if within ±24 hours. Otherwise, return an empty StaticDashboardData."""
    now = datetime.now()
    target_dt = now - timedelta(days=7)
    window_start_dt = target_dt - timedelta(days=1)
    window_end_dt = target_dt + timedelta(days=1)

    target_ticks = datetime_to_ticks(target_dt)
    window_start_ticks = datetime_to_ticks(window_start_dt)
    window_end_ticks = datetime_to_ticks(window_end_dt)

    conn = sqlite3.connect(DB_LOCATION)
    cur = conn.cursor()

    # date_retrieved is stored as INTEGER (ticks)
    cur.execute(
        """
        SELECT id, date_retrieved, properties
        FROM Dashboards
        WHERE date_retrieved >= ? AND date_retrieved <= ?
        ORDER BY ABS(date_retrieved - ?)
        LIMIT 1
        """,
        (window_start_ticks, window_end_ticks, target_ticks),
    )

    row = cur.fetchone()
    conn.close()

    if row is None:
        # No record within ±24h → return an empty StaticDashboardData
        return StaticDashboardData(
            list_repos=[],
        )

    id_, ticks_value, props_text = row
    record = DashboardRecordPy(
        id=id_,
        date_retrieved=ticks_to_datetime(int(ticks_value)),
        properties_text=props_text,
    )

    return dashboard_record_to_static_data(record)

def dashboard_record_to_static_data(record: DashboardRecordPy) -> StaticDashboardData:
    p = record.properties  # JSON dict from DashboardProperties

    sd = StaticDashboardData(
        total_lines_covered_count=p.get("TotalLinesCoveredCount", 0.0),
        total_repos_count=p.get("TotalReposCount", 0.0),
        total_classes_count=p.get("TotalClassesCount", 0.0),
        total_methods_count=p.get("TotalMethodsCount", 0.0),
        total_lines_count=p.get("TotalLinesCount", 0.0),
        average_line_coverage_percent=p.get("AverageLineCoveragePercent", 0.0),
        average_branch_coverage_percent=p.get("AverageBranchCoveragePercent", 0.0),
        total_branches_covered_count=p.get("TotalBranchesCount", 0.0),  # note spelling

        list_repos=p.get("ListRepos", []),

        list_hot_repos=p.get("HotRepos", []),
        list_healthy_repos=p.get("HealthyRepos", []),
        list_complex_methods=p.get("ComplexMethods", []),
        list_unhealthy_repos=p.get("UnhealthyRepos", []),

        coverlet_version=p.get("CoverletVersion", ""),
        dashboard_version=p.get("DashboardVersion", ""),
    )

    # Date fields: prefer JSON values if present, else DB record time
    date_retrieved_raw = p.get("DateRetrieved")
    data_age_raw = p.get("DataAge")

    if isinstance(date_retrieved_raw, str):
        sd.date_retrieved = datetime.fromisoformat(date_retrieved_raw)
    else:
        sd.date_retrieved = record.date_retrieved

    if isinstance(data_age_raw, str):
        sd.data_age = datetime.fromisoformat(data_age_raw)
    else:
        sd.data_age = record.date_retrieved

    return sd

def datetime_to_ticks(dt: datetime) -> int:
    delta = dt - DOTNET_EPOCH
    # compute in integers to avoid float precision
    ticks = (
        delta.days * 24 * 60 * 60 * DOTNET_TICKS_PER_SECOND
        + delta.seconds * DOTNET_TICKS_PER_SECOND
        + delta.microseconds * 10
    )
    return ticks

def ticks_to_datetime(ticks: int) -> datetime:
    # 1 tick = 100 ns = 0.1 µs → 10 ticks per microsecond
    return DOTNET_EPOCH + timedelta(microseconds=ticks / 10)

def upload_dashboard_to_database(dashboard: StaticDashboardData) -> None:
    """Insert a DashboardRecord row into the Dashboards table."""
    props_dict = static_dashboard_to_properties_dict(dashboard)
    props_json = json.dumps(props_dict)

    date_ticks = datetime_to_ticks(dashboard.date_retrieved)

    conn = sqlite3.connect(DB_LOCATION)
    cur = conn.cursor()

    try:
        cur.execute(
            """
            INSERT INTO Dashboards (
                date_retrieved,
                properties
            )
            VALUES (?, ?)
            """,
            (date_ticks, props_json),
        )

        conn.commit()
        print("Inserted dashboard snapshot into Dashboards table.")
    except Exception as ex:
        conn.rollback()
        print("Error while inserting dashboard:", ex)
        raise
    finally:
        conn.close()

def static_dashboard_to_properties_dict(d: StaticDashboardData) -> dict[str, Any]:
    """Build a DashboardProperties-shaped dict from StaticDashboardData."""

    props: dict[str, Any] = {
        # Totals / averages
        "TotalLinesCoveredCount": d.total_lines_covered_count,
        "TotalReposCount": d.total_repos_count,
        "TotalClassesCount": d.total_classes_count,
        "TotalMethodsCount": d.total_methods_count,
        "TotalLinesCount": d.total_lines_count,
        "AverageLineCoveragePercent": d.average_line_coverage_percent,
        "AverageBranchCoveragePercent": d.average_branch_coverage_percent,
        "TotalBranchesCoveredCount": d.total_branches_covered_count,
        "TotalLinesUncoveredCount": d.total_lines_uncovered_count,

        # Complex methods (derived)
        "TotalComplexMethodCount": d.complex_method_count,
        "AverageComplexMethodPercent": d.average_complex_method_percent,

        # Top 5 lists
        "HotRepos": [r.to_dict() for r in d.list_hot_repos],
        "ComplexMethods": [m.to_dict() for m in d.list_complex_methods],
        "HealthyRepos": [r.to_dict() for r in d.list_healthy_repos],
        "UnhealthyRepos": [r.to_dict() for r in d.list_unhealthy_repos],

        # Given properties
        "DateRetrieved": ensure_datetime(d.date_retrieved).isoformat(),
        "DataAge": ensure_datetime(d.data_age).isoformat(),
        "CoverletVersion": d.coverlet_version,
        "DashboardVersion": d.dashboard_version,

        # Full repo list
        "ListRepos": [repo.to_dict() for repo in d.list_repos],
    }

    return props

def ensure_datetime(value):
    """Ensure the value is a datetime. Converts ints/unix timestamps/dotnet ticks."""
    if isinstance(value, datetime):
        return value

    # .NET ticks?
    if isinstance(value, int) and value > 10**12:
        return ticks_to_datetime(value)

    # Unix timestamp?
    if isinstance(value, int):
        return datetime.fromtimestamp(value)

    # Strings
    if isinstance(value, str):
        try:
            return datetime.fromisoformat(value)
        except ValueError:
            pass

    # fallback
    return datetime.fromtimestamp(0)

if __name__ == "__main__":
    args = sys.argv[1:]

    if len(args) == 0:
        UPLOAD_TO_DB_BOOL = False
    else:
        if args[0] == "--bypass-console":
            UPLOAD_TO_DB_BOOL = True
            print("Bypassing human input, uploading to database...")

    start = time.perf_counter()
    Execute()
    end = time.perf_counter()

    print(f"Execute() took {end - start:.4f} seconds")
