﻿using System.ComponentModel.DataAnnotations;

namespace HyperCache.Api.Models;

public class CustomProperty
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    public required string Name { get; set; }
    public required string Value { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public required string CreatedBy { get; set; }
    public required string ModifiedBy { get; set; }
    public required string ParentTable { get; set; }
    [Timestamp]
    public byte[] RowVersion { get; set; } = [];
}