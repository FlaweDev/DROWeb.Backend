using DROWeb.Domain;
using FluentValidation;

namespace DROWeb.WebAPI.Endpoints.GetVersion;

public record GetVersionRequest();

public record CreatePlayerResponse(string version);