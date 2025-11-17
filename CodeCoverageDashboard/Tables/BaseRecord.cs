// COPYRIGHT © 2025 ESRI
//
// TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
// Unpublished material - all rights reserved under the
// Copyright Laws of the United States.
//
// For additional information, contact:
// Environmental Systems Research Institute, Inc.
// Attn: Contracts Dept
// 380 New York Street
// Redlands, California, USA 92373
//
// email: contracts@esri.com

namespace CodeCoverageDashboard.Tables;

public abstract class BaseRecord<T>
{
    [Column("id")]
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Column("date_retrieved")]
    public DateTime DateRetrieved { get; set; } = DateTime.MinValue;

    [Column("properties")]
    public string PropertiesText { get; set; } = string.Empty;

    [Ignore]
    public T? Properties
    {
        get => JsonSerializer.Deserialize<T>(this.PropertiesText);
        set => this.PropertiesText = JsonSerializer.Serialize(value);
    }
}
