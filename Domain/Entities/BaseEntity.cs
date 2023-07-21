﻿namespace MyDisk.Domain.Entities;

public abstract class BaseEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}