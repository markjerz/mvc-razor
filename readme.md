# ASP.Net Mvc using Razor Components

This is a proof of concept to show that you can build an ASP.Net MVC app using razor components for the UI.

## Why do this?

The key motivation is that the syntax provided with razor components is much cleaner than Partials, TagHelpers and View Components. 
In razor, the components are strongly typed and, with multiple child components allowed, enable composition of simple code (See [Blazor templated components](https://learn.microsoft.com/en-gb/aspnet/core/blazor/components/templated-components?view=aspnetcore-8.0#templated-components) for more information on that.)

The next question is "Why don't you just use Blazor SSR?". This is tricky - essentially:

* I like (and am familiar with) the MVC way of doing things. 
* I like using Url.Action (and similar) to generate URLs.
* Similarly, I don't understand how /page works, how I can intercept that?
* I'm a fan of Html-over-the-wire JS libraries like Turbo and HTMX and while enhanced navigation in Blazor SSR does some of this, it's too magical/not as good.
* Similarly, I think that WASM is more complicated (than a turbo/htmx approach) and that Blazor server with SignalR sounds complex/error prone.
* The jump from MVC (with razor view engine) to MVC (using razor components) is much smaller.

As a result, I believe that the productivity of this approach, with the combination of turbo.hotwired.dev for interactivity hits the sweet spot!

This is based on the sorts of things I build - for example, when large amounts of client interactivity are required I'd combine this approach with WASM.

## What does this sample show?

### Return razor component from MVC action

If you look at FooController you can see that we return `RazorView<T>()` from the action methods.

```
public IResult Index(bool hideSidebar = false) {
    return this.RazorView<Foo>(new { hideSidebar });
}
```

`RazorView()` is a simple wrapper around `new RazorComponentResult<T>()` which makes the syntax feel more familiar.

### How do the views work?

As a proof of concept, in the `Foo.razor` component, I've tried to replicate the features that you would use in the normal MVC razor view engine:

#### Using `Url.Action()`

In `Program.cs` we register an IUrlHelper service in to the DI, so that you can use the MVC routing in razor views.

```
@inject IUrlHelper Url
...
<a href="@Url.Action("Index", new { hideSidebar = !HideSidebar })">
```

#### Using Layout's and passing parameters to them

By using `<LayoutView>` you can execute code in the page before execution of the layout. 
Couple that with a state container registered via DI and you can pass arbitrary parameters to the layout.

```
public class LayoutState {
    public bool DisableSidebar { get; set; }
}
```

Program.cs
```
builder.Services.AddScoped<LayoutState>();
```

Foo.razor
```
@inject LayoutState LayoutState
@{ LayoutState.DisableSidebar = HideSidebar; }

<LayoutView Layout="typeof(MainLayout)">
...
```

MainLayout.razor
```
@inject LayoutState LayoutState
...
@if (!LayoutState.DisableSidebar)
```

#### Accessing the HttpContext from the razor component

That's very simple. Just register the HttpContextAccessor `builder.Services.AddHttpContextAccessor();`
and then inject in to the component `@inject IHttpContextAccessor HttpContextAccessor`

#### Form validation

This is a bit trickier. As you can't use HtmlHelper in a razor component we need to rebuild some of the helper methods to make validation of forms easier.

In the post method, we have the standard ModelState.IsValid pattern:

```
[HttpPost]
[ValidateAntiForgeryToken]
public IResult Post(FooModel model) {
    if (!ModelState.IsValid) {
        return this.RazorView<Foo>();
    }
            
    return this.RazorView<Foo>(new { Model = model });
}
```

And then we can access that in the UI using the `IActionContextAccessor` which provides access to the ModelState.

I've added illustrative components for that with `<ErrorSummary>` and `<ErrorMessage>`

ErrorMessage.razor (for example)
```
@using Microsoft.AspNetCore.Mvc.Infrastructure
@inject IActionContextAccessor ActionContextAccessor

@if (ActionContextAccessor.ActionContext?.ModelState.TryGetValue(Name, out var entry) ?? false) {
    foreach (var error in entry.Errors) {
        <p>@error.ErrorMessage</p>
    }    
}

@code {
    [Parameter]
    public string Name { get; set; }
}
```