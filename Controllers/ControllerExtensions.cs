using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MvcRazor.Controllers;

public static class ControllerExtensions
{
    public static IResult RazorView<T>(this Controller controller) where T : IComponent
    {
        return new RazorComponentResult<T>();
    }

    public static IResult RazorView<T>(this Controller controller, IReadOnlyDictionary<string, object?> parameters) where T : IComponent
    {
        return new RazorComponentResult<T>(parameters);
    }

    public static IResult RazorView<T>(this Controller controller, object parameters) where T : IComponent
    {
        return new RazorComponentResult<T>(parameters);
    }

    public static IResult RazorView(this Controller controller, Type componentType)
    {
        return new RazorComponentResult(componentType);
    }

    public static IResult RazorView(this Controller controller, Type componentType, IReadOnlyDictionary<string, object?> parameters) 
    {
        return new RazorComponentResult(componentType, parameters);
    }

    public static IResult RazorView(this Controller controller, Type componentType, object parameters) 
    {
        return new RazorComponentResult(componentType, parameters);
    }
}