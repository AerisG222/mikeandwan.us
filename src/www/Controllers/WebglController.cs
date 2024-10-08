﻿using Microsoft.AspNetCore.Mvc;
using MawMvcApp.ViewModels.Navigation;

namespace MawMvcApp.Controllers;

[Route("webgl")]
public class WebglController
    : MawBaseController<WebglController>
{
    public WebglController(ILogger<WebglController> log)
        : base(log)
    {

    }

    [HttpGet("")]
    public IActionResult Index()
    {
        ViewBag.NavigationZone = NavigationZone.Webgl;
        return View();
    }

    [HttpGet("cube")]
    public IActionResult Cube()
    {
        ViewBag.NavigationZone = NavigationZone.Webgl;
        return View();
    }

    [HttpGet("text")]
    public IActionResult Text()
    {
        ViewBag.NavigationZone = NavigationZone.Webgl;
        return View();
    }

    [HttpGet("shader")]
    public ActionResult Shader()
    {
        ViewBag.NavigationZone = NavigationZone.Webgl;
        return View();
    }

    [HttpGet("blender-model")]
    public ActionResult BlenderModel()
    {
        ViewBag.NavigationZone = NavigationZone.Webgl;
        return View();
    }
}
