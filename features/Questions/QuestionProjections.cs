using System;
using System.Linq.Expressions;

namespace PastPapers.ContentApi.Features.Questions;

public static class QuestionProjections
{
    public static readonly Expression<Func<Question, QuestionResponse>>
        ToResponse = question => new QuestionResponse(
            question.Id,
            question.QuestionNumber,
            question.DisplayOrder,
            question.ExamYear,
            question.ExamSeason,
            question.PaperNumber,
            question.QuestionImageUrl,
            question.MemoImageUrl,
            question.TopicId,
            question.Topic.Name,
            question.Topic.Slug,
            question.Topic.Grade,
            question.Topic.SubjectId,
            question.Topic.Subject.Name,
            question.Topic.Subject.Slug);
}