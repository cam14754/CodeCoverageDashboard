from __future__ import annotations
import os
import time
from dataclasses import dataclass, field
from datetime import datetime
from typing import List, Optional
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


@dataclass
class MethodData:
    name: Optional[str] = "Unknown Name"
    errors: List[str] = field(default_factory=list)

    coverage_percent: float = 0.0
    branch_coverage_percent: float = 0.0

    complexity: float = 0.0
    list_lines: List["LineData"] = field(default_factory=list)

    signature: Optional[str] = "()"


@dataclass
class LineData:
    line_number: Optional[float] = None
    hits: Optional[float] = None


# ==============================================
# DTOs (xsdata):
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


# ==============================================
# Functions
# ==============================================

DBLocation = r"C:\Users\cam14754\Desktop\UnitTestingInternProject\CodeCoverageDashboard\CodeCoverageDashboard\CodeCoverageDataBase.db"
XMLLocation = r"C:\Users\cam14754\Desktop\Reports"


def Execute():
    latest_full_path = GetLatestReportDir()
    if latest_full_path is None:
        return

    list_coverages = ParseXML(latest_full_path)

    list_repo_datas = AnalyzeCoverageDTO(list_coverages)

    for x in list_repo_datas:
        print(x)


def GetLatestReportDir():
    repo_dir_list = os.listdir(XMLLocation)

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

    latest_full_path = os.path.join(XMLLocation, latest_dirname)

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
        print("Parsed:", coverage.packages[0].name)
        coverage_list.append(coverage)

    return coverage_list

def AnalyzeCoverageDTO(coverage_list: List["CoverageDTO"]) -> List["RepoData"]:
    repodata: List["RepoData"] = []

    for coverage in coverage_list:
        data = RepoData()

        package: PackageDTO = coverage.packages[0]

        #Meta Data
        data.name = package.name
        data.date_retrieved = coverage.timestamp

        #Root Attributes
        data.covered_lines = coverage.lines_covered
        data.total_lines = coverage.lines_covered
        data.coverage_percent = coverage.line_rate
        data.branch_rate = coverage.branch_rate
        data.total_branches = coverage.branches_valid
        data.total_covered_branches = coverage.branches_covered
        data.uncovered_lines = data.total_lines - data.uncovered_lines

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
                    )
                )

            # Order methods by name
            list_methods = sorted(list_methods, key=lambda m: m.name)

            # Compute TotalLines and CoveredLines like your LINQ
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
        list_classes = sorted(list_classes, key=lambda c: c.name)

        # Assign back to repo_data
        data.list_classes = list_classes

        repodata.append(data)

    return repodata

if __name__ == "__main__":
    start = time.perf_counter()
    Execute()
    end = time.perf_counter()

    print(f"Execute() took {end - start:.4f} seconds")
