namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);
           Console.WriteLine(RemoveBooks(db));

            //IncreasePrices(db);
        }

        //--------------------------------------------------15--------------------------------------------------
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.Copies < 4200)
                .ToList();

            foreach (var bk in books)
            {
                context.Books.Remove(bk);
            }

            int deletedBooks = context.SaveChanges();

            return books.Count;
        }

        //--------------------------------------------------14--------------------------------------------------
        public static void IncreasePrices(BookShopContext context)
        {
            var booksI = context.Books
                .Where(x => x.ReleaseDate.Value.Year < 2010)
                .ToList();

            foreach (var book in booksI)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        //--------------------------------------------------13--------------------------------------------------
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var books = context.Categories
                .Select(c => new
                {
                    c.Name,
                    Books = c.CategoryBooks.Select(cb => new
                    {
                        cb.Book.Title,
                        cb.Book.ReleaseDate
                    })
                    .OrderByDescending(x => x.ReleaseDate)
                    .Take(3)
                    .ToList()
                })
                .OrderBy(x => x.Name)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"--{book.Name}");

                foreach (var item in book.Books)
                {
                    sb.AppendLine($"{item.Title} ({item.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }
        //--------------------------------------------------12--------------------------------------------------
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var getTotalProfit = context.Categories
                .Select(categ => new
                {
                    categ.Name,
                    Profit = categ.CategoryBooks.Sum(s => s.Book.Price * s.Book.Copies)
                })
                .OrderByDescending(p => p.Profit)
                .ThenBy(n => n.Name)
                .ToList();

            var result = string.Join(Environment.NewLine, getTotalProfit.Select(x => $"{x.Name} ${x.Profit:F2}"));

            return result;

        }

        //--------------------------------------------------11--------------------------------------------------
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var copyesByAuthor = context.Authors
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName,
                    TotalCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(t => t.TotalCopies)
                .ToList();

            var result = string.Join(Environment.NewLine, copyesByAuthor.Select(x => $"{x.FirstName} {x.LastName} {x.TotalCopies}"));

            return result;
        }
        //--------------------------------------------------10--------------------------------------------------
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int number = lengthCheck;

            var booksCount = context.Books
                .Where(b => b.Title.Length > number)
                .Select(x => x.Title)
                .ToList()
                .Count();

            //int result = booksCount;

            return booksCount;
        }

        //--------------------------------------------------9--------------------------------------------------
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {

            var str = input.ToLower();
            var searchBookByAuthor = context.Books
                .Include(x => x.Author)
                .Where(a => a.Author.LastName.StartsWith(str))
                .Select(x => new
                {
                    x.Title,
                    x.BookId
                })
                .OrderBy(x => x.BookId)
                .ToList();

            var result = string.Join(Environment.NewLine, searchBookByAuthor.Select(x => $"{x.Title}"));

            return result;
        }

        //--------------------------------------------------8--------------------------------------------------
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var con = input.ToLower();

            var bookTitle = context.Books
                .Where(bt => bt.Title.Contains(con))
                .Select(x => new
                {
                    x.Title
                })
                .OrderBy(x => x.Title)
                .ToList();

            var result = string.Join(Environment.NewLine, bookTitle.Select(x => $"{x.Title}"));

            return result;
        }

        //--------------------------------------------------7--------------------------------------------------
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {

            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(n => new
                {
                    n.FirstName,
                    n.LastName
                })
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.FirstName)
                .ToList();

            var result = string.Join(Environment.NewLine, authors.Select(x => $"{x.FirstName} { x.LastName}"));

            return result;

        }

        //--------------------------------------------------6--------------------------------------------------
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var dateConverted = DateTime.ParseExact(date, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

            var bookReleasedBefore = context.Books
                .Where(rb => rb.ReleaseDate < dateConverted)
                .Select(x => new
                {
                    x.Title,
                    x.Price,
                    x.EditionType,
                    x.ReleaseDate
                })
                .OrderByDescending(x => x.ReleaseDate)
                .ToList();

            var result = string.Join(Environment.NewLine, bookReleasedBefore.Select(x => $"{x.Title} - {x.EditionType} - ${x.Price:F2}"));

            return result;
        }

        //--------------------------------------------------5--------------------------------------------------
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var inputsCategories = input
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToLower())
                .ToList();

            var booksByCategiry = context.Books
                .Include(x => x.BookCategories)
                .ThenInclude(x => x.Category)
                .Where(book => book.BookCategories.Any(booksByCategiry => inputsCategories.Contains(booksByCategiry.Category.Name.ToLower())))
                .Select(book => new
                {
                    book.Title
                })
                .OrderBy(b => b.Title)
                .ToList();

            var sb = new StringBuilder();

            foreach (var bk in booksByCategiry)
            {
                sb.AppendLine($"{bk.Title}");
            }
            return sb.ToString().TrimEnd();

        }
        //--------------------------------------------------4--------------------------------------------------
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var notReleasedBooks = context.Books
                .Where(nrb => nrb.ReleaseDate.Value.Year != year)
                .Select(x => new
                {
                    x.Title,
                    x.BookId
                })
                .OrderBy(x => x.BookId)
                .ToList();

            var sb = new StringBuilder();
            foreach (var book in notReleasedBooks)
            {
                sb.AppendLine($"{book.Title}");
            }
            return sb.ToString().TrimEnd();
        }

        //--------------------------------------------------3--------------------------------------------------
        public static string GetBooksByPrice(BookShopContext context)
        {
            var booksByPrice = context.Books
                .Where(bbp => bbp.Price > 40)
                .Select(x => new
                {
                    x.Title,
                    x.Price
                })
                .OrderByDescending(x => x.Price)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in booksByPrice)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();

        }

        //--------------------------------------------------2--------------------------------------------------
        public static string GetGoldenBooks(BookShopContext context)
        {
            var booksGoldenEdituion = context.Books
                .Where(geb => geb.Copies < 5000 && geb.EditionType == EditionType.Gold)
                .Select(x => new
                {
                    x.Title,
                    x.BookId
                })
                .OrderBy(x => x.BookId)
                .ToList();

            var sb = new StringBuilder();
            foreach (var book in booksGoldenEdituion)
            {
                sb.AppendLine($"{book.Title}");
            }
            return sb.ToString().TrimEnd();
        }
        //--------------------------------------------------1--------------------------------------------------
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestriction = Enum.Parse<AgeRestriction>(command, true);
            //var ageRestriction1 = (AgeRestriction)Enum.Parse(typeof(AgeRestriction) , command, true);

            var booksWithRestriction = context.BooksCategories
                .Where(r => r.Book.AgeRestriction == ageRestriction)
                .Select(x => new
                {
                    x.Book.Title
                })
                .OrderBy(x => x.Title)
                .ToArray();

            var sb = new StringBuilder();

            //var result = string.Join(Environment.NewLine, booksWithRestriction);

            foreach (var bar in booksWithRestriction)
            {
                sb.AppendLine($"{bar.Title}");
            }

            return sb.ToString().TrimEnd();

        }
    }
}
