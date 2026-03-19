library system
Task: Build a library system using a 3-tier architecture
that allows users to search for books, borrow books, and return books.
Use .NET for the presentation layer andbusiness logic layer, and ADO.NET for the data access layer.
The database schema should include tables for books, users, and borrowings.
Requirements:

1. The presentation layer should allow users to search for books by title, author, or ISBN. It
should display a list of matching books, along with their availability status (available or
checked out).
2. The presentation layer should allow users to borrow books by selecting them from the list of
search results. If a book is not available, the user should be informed and not allowed to
borrow it.
3. The presentation layer should allow users to return books that they have borrowed. Once a
book is returned, its availability status should be updated in the database.
4. The business logic layer should handle the search, borrow, and return operations, as well as
the validation of user input.
5. The data access layer should interact with the database to retrieve and update information
about books, users, and borrowings.
Tasks:
6. Design the database schema for the library system, including tables for books, users, and
borrowings.
7. Implement the data access layer using ADO.NET to interact with the database.
• Implement a repository pattern for accessing data in the database.
• Implement the methods for retrieving and updating information about books, users,
and borrowings.
8. Implement the business logic layer to handle the search, borrow, and return operations, as
well as the validation of user input.
• Implement a service or manager class that provides the necessary functionality to
the presentation layer.
• Implement the methods for searching for books, borrowing books, and returning
books.
9. Implement the presentation layer to allow users to search for books, borrow books, and
return books.
• Implement a user interface that allows users to search for books by title, author, or
ISBN.
• Implement a user interface that allows users to borrow books by selecting them
from the list of search results.
• Implement a user interface that allows users to return books that they have
borrowed.
• Implement validation of user input to prevent invalid input or data errors.
Considerations:
• Entity framework is not allowed.
• Common Errors should be handled, for example (connection error, etc.…).
• The application should be customer oriented so that it functions exactly as the “real” will.
Notes:
ISBN stands for International Standard Book Number. It is a unique identifier assigned to books and
other monographic publications to help identify them and make them easily searchable in
databases, libraries, and bookstores.
