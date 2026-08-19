using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PastPapers.ContentApi.Features.Questions;

public static class CreateQuestionRequestValidator
{
	private static readonly Regex SlugPattern = new(
		"^[a-z0-9]+(?:-[a-z0-9]+)*$",
		RegexOptions.Compiled);

	public static Dictionary<string, string[]> Validate(
		CreateQuestionRequest request)
	{
		var errors = new Dictionary<string, string[]>();

		ValidateSlug(
			request.SubjectSlug,
			"subjectSlug",
			"Subject slug",
			errors);

		ValidateSlug(
			request.TopicSlug,
			"topicSlug",
			"Topic slug",
			errors);

		if (request.Grade is < 10 or > 12)
		{
			errors["grade"] = ["Grade must be between 10 and 12."];
		}

		if (request.ExamYear is < 1996 or > 2100)
		{
			errors["examYear"] =
				["Exam year must be between 1996 and 2100."];
		}

		if (string.IsNullOrWhiteSpace(request.ExamSeason) ||
			request.ExamSeason.Length > 50)
		{
			errors["examSeason"] =
				["Exam season is required and may not exceed 50 characters."];
		}

		if (request.PaperNumber is < 1 or > 4)
		{
			errors["paperNumber"] =
				["Paper number must be between 1 and 4."];
		}

		if (string.IsNullOrWhiteSpace(request.QuestionNumber) ||
			request.QuestionNumber.Length > 30)
		{
			errors["questionNumber"] =
				["Question number is required and may not exceed 30 characters."];
		}

		if (request.DisplayOrder < 1)
		{
			errors["displayOrder"] =
				["Display order must be at least 1."];
		}

		ValidateHttpsUrl(
			request.QuestionImageUrl,
			"questionImageUrl",
			errors);

		ValidateHttpsUrl(
			request.MemoImageUrl,
			"memoImageUrl",
			errors);

		return errors;
	}

	private static void ValidateSlug(
		string? value,
		string field,
		string label,
		IDictionary<string, string[]> errors)
	{
		if (string.IsNullOrWhiteSpace(value) ||
			value.Length > 150 ||
			!SlugPattern.IsMatch(value))
		{
			errors[field] =
			[
				$"{label} must be a lowercase, hyphen-separated slug."
			];
		}
	}

	private static void ValidateHttpsUrl(
		string? value,
		string field,
		IDictionary<string, string[]> errors)
	{
		var isValid = Uri.TryCreate(
						  value,
						  UriKind.Absolute,
						  out var uri) &&
					  uri.Scheme == Uri.UriSchemeHttps;

		if (!isValid)
		{
			errors[field] = ["A valid HTTPS URL is required."];
		}
	}
}