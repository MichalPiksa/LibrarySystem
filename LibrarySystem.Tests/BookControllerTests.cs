using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using LibrarySystem.Models;
using LibrarySystem.Controllers;

namespace LibrarySystem.Tests;

public class BookControllerTests
{
    private void ResetBooks(params Book[] initial)
    {
        var list = new List<Book>(initial);
        var field = typeof(BooksController).GetField("books", BindingFlags.Static | BindingFlags.NonPublic);
        field.SetValue(null, list);
    }

    [Fact]
    public void GetBooks_ReturnsAllBooks()
    {
        // Arrange
        var b1 = new Book { Id = Guid.NewGuid(), Title = "A", Author = "a", PublishedYear = 2000 };
        var b2 = new Book { Id = Guid.NewGuid(), Title = "B", Author = "b", PublishedYear = 2001 };
        ResetBooks(b1, b2);
        var controller = new BooksController();

        // Act
        var actionResult = controller.GetBooks();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var list = Assert.IsType<List<Book>>(ok.Value);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void GetBookById_ExistingId_ReturnsBook()
    {
        // Arrange
        var id = Guid.NewGuid();
        var book = new Book { Id = id, Title = "X", Author = "Y", PublishedYear = 1999 };
        ResetBooks(book);
        var controller = new BooksController();

        // Act
        var actionResult = controller.GetBookById(id);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returned = Assert.IsType<Book>(ok.Value);
        Assert.Equal(id, returned.Id);
    }

    [Fact]
    public void CreateBook_AddsBook_ReturnsCreated()
    {
        // Arrange
        ResetBooks();
        var controller = new BooksController();
        var newBook = new Book { Title = "New", Author = "Author", PublishedYear = 2023 };

        // Act
        var result = controller.CreateBook(newBook);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result);
        var createdValue = Assert.IsType<Book>(created.Value);
        Assert.NotEqual(Guid.Empty, createdValue.Id);

        var field = typeof(BooksController).GetField("books", BindingFlags.Static | BindingFlags.NonPublic);
        var list = (List<Book>)field.GetValue(null);
        Assert.Contains(list, b => b.Id == createdValue.Id);
    }

    [Fact]
    public void UpdateBook_ExistingId_UpdatesFields()
    {
        // Arrange
        var id = Guid.NewGuid();
        var original = new Book { Id = id, Title = "Old", Author = "OldA", PublishedYear = 1990 };
        ResetBooks(original);
        var controller = new BooksController();
        var updated = new Book { Title = "New", Author = "NewA", PublishedYear = 2020 };

        // Act
        var result = controller.UpdateBook(id, updated);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<Book>(ok.Value);
        Assert.Equal("New", returned.Title);
        Assert.Equal("NewA", returned.Author);
        Assert.Equal(2020, returned.PublishedYear);
    }

    [Fact]
    public void DeleteBook_ExistingId_RemovesBook()
    {
        // Arrange
        var id = Guid.NewGuid();
        var book = new Book { Id = id, Title = "ToDelete", Author = "A", PublishedYear = 2005 };
        ResetBooks(book);
        var controller = new BooksController();

        // Act
        var result = controller.DeleteBook(id);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var field = typeof(BooksController).GetField("books", BindingFlags.Static | BindingFlags.NonPublic);
        var list = (List<Book>)field.GetValue(null);
        Assert.DoesNotContain(list, b => b.Id == id);
    }
}
