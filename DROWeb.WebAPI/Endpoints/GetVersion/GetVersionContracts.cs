using DROWeb.Domain;
using FluentValidation;

namespace DROWeb.WebAPI.Endpoints.GetVersion;

public record CreatePlayerResponse(string version);