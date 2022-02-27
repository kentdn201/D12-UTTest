using System;
using System.Collections.Generic;
using System.Linq;
using D5.Controllers;
using D7.Models;
using D7.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace MVC.Tests;

public class RookiesControllerTests
{
    private Mock<ILogger<RookiesController>> _loggerMock;

    private Mock<IPersonService> _personServiceMock;

    static List<Person> _people = new List<Person>
    {
            new Person
            {
                FirstName = "Nam",
                LastName = "Nguyen Thanh",
                Gender = "Male",
                DateOfBirth = new DateTime(2001, 1, 20),
                PhoneNumber = "",
                BirthPlace = "Ha Noi",
                IsGraduated = false
            },
            new Person
            {
                FirstName = "Nam",
                LastName = "Nguyen Thanh",
                Gender = "Male",
                DateOfBirth = new DateTime(2001, 1, 20),
                PhoneNumber = "",
                BirthPlace = "Ha Noi",
                IsGraduated = false
            },
            new Person
            {
                FirstName = "Nam",
                LastName = "Nguyen Thanh",
                Gender = "Male",
                DateOfBirth = new DateTime(2001, 1, 20),
                PhoneNumber = "",
                BirthPlace = "Ha Noi",
                IsGraduated = false
            }
    };

    // Set up
    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<RookiesController>>();
        _personServiceMock = new Mock<IPersonService>();

        // Setup
        _personServiceMock.Setup(s => s.GetAll()).Returns(_people);
    }

    // Test
    [Test]
    public void Index_ReturnViewResult_WithAListOfPeople()
    {
        // Arrange
        var controller = new RookiesController(_loggerMock.Object, _personServiceMock.Object);
        var expectedCount = _people.Count;

        // Act
        var result = controller.Index();

        // Assert
        Assert.IsInstanceOf<ViewResult>(result, "Invalid return type.");

        var view = (ViewResult)result;
        Assert.IsAssignableFrom<List<Person>>(view.ViewData.Model, "Invalid view data model");

        var model = view.ViewData.Model as List<Person>;
        Assert.IsNotNull(model, "View data must not be null");
        Assert.AreEqual(expectedCount, model?.Count, "Model count not equals to expected count.");

        // var firstPerson = model?.First();
        // Assert.AreEqual("Nguyen Thanh Nam", firstPerson?.FullName, "Name not equals");
    }

    [Test]
    public void Detail_AValidIndex_ReturnsViewResult_WithAPerson()
    {
        // Setup
        const int index = 2;
        _personServiceMock.Setup(x => x.GetOne(index)).Returns(_people[index - 1]);
        var expected = _people[index - 1];

        // Arrange
        var controller = new RookiesController(_loggerMock.Object, _personServiceMock.Object);

        // Act
        var result = controller.Detail(index);

        // Assert
        Assert.IsInstanceOf<ViewResult>(result, "Invalid return type.");

        var view = (ViewResult)result;
        Assert.IsAssignableFrom<Person>(view.ViewData.Model, "Invalid view data model");

        var model = view.ViewData.Model as Person;
        Assert.IsNotNull(model, "View data must not be null");
        Assert.AreEqual(expected, model, "Model count not equals to expected count.");
    }

    [Test]
    public void Detail_InValidIndex_ReturnsNotFoundObjectResult_WithAStringMessage()
    {
        // Setup
        const int index = 20;
        const string message = "Index out of range";
        _personServiceMock.Setup(x => x.GetOne(index)).Throws(new IndexOutOfRangeException(message));
        // var expected = _people[index - 1];

        // Arrange
        var controller = new RookiesController(_loggerMock.Object, _personServiceMock.Object);

        // Act
        var result = controller.Detail(index);

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result, "Invalid return type.");

        var view = result as NotFoundObjectResult;
        Assert.IsNotNull(view, "View data must not be null");
        Assert.IsInstanceOf<string>(view?.Value, "Invalid data type");

        Assert.AreEqual(message, view?.Value?.ToString(), "Not equal!!!!!!!!");
    }

    [Test]
    public void Create_InvalidModel_ReturnView_WithErrorInModelState()
    {
        const string key = "Error";
        const string message = "Invalid model";

        // Arrange
        var controller = new RookiesController(_loggerMock.Object, _personServiceMock.Object);
        controller.ModelState.AddModelError(key, message);

        // Act
        var result = controller.Create(null);

        // Assert
        Assert.IsInstanceOf<ViewResult>(result, "Invalid return type.");

        var view = (ViewResult)result;
        var modelState = view.ViewData.ModelState;

        Assert.IsFalse(modelState.IsValid, "Invalid model state");
        Assert.AreEqual(1, modelState.ErrorCount, "");

        modelState.TryGetValue(key, out var value);
        var error = value?.Errors.First().ErrorMessage;
        Assert.AreEqual(message, error);
    }

    [Test]
    public void Create_ValidModel_ReturnsRedirectToActionIndex()
    {
        // Setup
        var person = new Person
        {
            FirstName = "Nam New",
            LastName = "Nguyen Thanh",
            Gender = "Male",
            DateOfBirth = new DateTime(2001, 1, 20),
            PhoneNumber = "",
            BirthPlace = "Ha Noi",
            IsGraduated = false
        };
        _personServiceMock.Setup(x => x.Create(person)).Callback<Person>((Person p) =>
        {
            _people.Add(p);
        });

        // Arrange
        var controller = new RookiesController(_loggerMock.Object, _personServiceMock.Object);
        var expected = _people.Count + 1;

        // Act
        var result = controller.Create(person);

        // Assert
        Assert.IsInstanceOf<RedirectToActionResult>(result, "Invalid return type.");

        var view = (RedirectToActionResult)result;
        Assert.AreEqual("Index", view.ActionName, "Invalid action name");

        var actual = _people.Count;
        Assert.AreEqual(expected, actual, "Not equal");

        Assert.AreEqual(person, _people.Last(), "Not equal");
    }

    [Test]
    public void Remove_ValidModel_ReturnRedirectToActionIndex()
    {
        // Setup
        var id = 1;
        _personServiceMock.Setup(x => x.Delete(id)).Callback(() => _people.RemoveAt(id));

        // Arrange
        var controller = new RookiesController(_loggerMock.Object, _personServiceMock.Object);
        var expected = _people.Count - 1;

        // Act
        var result = controller.Delete(id);

        // Assert
        Assert.IsInstanceOf<RedirectToActionResult>(result, "Invalid return type.");

        var view = (RedirectToActionResult)result;
        Assert.AreEqual("Index", view.ActionName, "Invalid action name");

        var actual = _people.Count;
        Assert.AreEqual(expected, actual, "Not equal");
    }
    
    // Free memory(Giải phóng bộ nhớ)
    [TearDown]
    public void TearDown()
    {
        // _loggerMock = null;
        // _personServiceMock = null;
    }
}
