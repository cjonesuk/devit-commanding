using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Devit.Commanding.AspNetCore;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        throw new NotImplementedException();
    }

    public static IActionResult ToActionResult<TResponse>(this Result<TResponse> result)
    {
        throw new NotImplementedException();
    }
}
