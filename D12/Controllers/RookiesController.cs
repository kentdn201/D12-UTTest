using D7.Models;
using D7.Services;
using Microsoft.AspNetCore.Mvc;

namespace D5.Controllers;

[Route("")]
[Route("Nashtech")]
public class RookiesController : Controller
{
    private readonly IPersonService _personService;

    public RookiesController(IPersonService personService)
    {
        _personService = personService;
    }

    [Route("rookies")]
    public IActionResult Index()
    {
        var people = _personService.GetAll();
        return View(people);
    }

    [Route("rookies/create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Route("rookies/create")]
    public IActionResult Create(Person model)
    {
        if (!ModelState.IsValid) return View();

        _personService.Create(model);
        // return Json(model);
        return RedirectToAction("Index");
    }

    [Route("rookies/edit")]
    public IActionResult Edit(int index)
    {
        try
        {
            var person = _personService.GetOne(index);
            ViewBag.PersonIndex = index;
            return View(person);
        }
        catch (System.Exception)
        {
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [Route("rookies/edit")]
    public IActionResult Edit(int index, Person model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.PersonIndex = index;
            return View();
        }

        _personService.Update(index, model);
        // return Json(model);
        return RedirectToAction("Index");
    }

    [Route("rookies/delete")]
    public IActionResult Delete(int index)
    {
        try
        {
            _personService.Delete(index);
        }
        catch (System.Exception)
        {

        }

        return RedirectToAction("Index");
    }


    [Route("rookies/detail")]
    public IActionResult Detail(int index)
    {
        try
        {
            var person = _personService.GetOne(index);
            ViewBag.PersonIndex = index;
            return View(person);
        }
        catch (System.Exception)
        {
            return RedirectToAction("Index");
        }
    }

    [Route("rookies/result")]
    public IActionResult DeleteWithResult(int index)
    {
        try
        {
            var person = _personService.GetOne(index);
            HttpContext.Session.SetString("DELETED_USER_NAME", person.FullName);
            _personService.Delete(index);
        }
        catch (System.Exception)
        {

        }

        return RedirectToAction("Result");
    }

    public IActionResult Result()
    {
        var deletedUserName = HttpContext.Session.GetString("DELETED_USER_NAME");
        ViewBag.DeletedUserName = deletedUserName;
        return View();
    }
}