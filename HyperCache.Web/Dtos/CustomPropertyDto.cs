﻿namespace HyperCache.Web.Dtos;

public class CustomPropertyDto
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
}