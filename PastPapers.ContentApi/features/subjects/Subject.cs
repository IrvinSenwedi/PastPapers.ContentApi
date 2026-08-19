using System;
using System.Collections.Generic;
using PastPapers.ContentApi.Features.Topics;
namespace PastPapers.ContentApi.Features.Subjects;

public sealed class Subject
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Slug { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<Topic> Topics { get; set; } = [];
}