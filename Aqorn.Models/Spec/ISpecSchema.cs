﻿namespace Aqorn.Models.Spec;

/// <summary>
/// Base model for a specification file.
/// </summary>
public interface ISpecSchema
{
    IColumnSpec[] Parameters { get; }
    ITableSpec[] Tables { get; }
}