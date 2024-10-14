# Library

This is a RESTful API project developed entirely upon C# on the Visual Studio IDE. It uses, 'Insomnia' as API client, and SQL Server as database.

'Library' as its name implies, is an API that serves as a library in a real-life scenario. It focuses on optimizing the main objective of a library, the book loan system. To carry out this service, a user management environment is implemented.

The real goal behind this web API is to demonstrate my skills as a C# web developer for any potential employer.

## Table Of Contents

1. 

## Installation

## Features

- Redis (Cache)
- Authentication
- JSON Web Token (JWT)
- Error Handling
- Serilog (Logging Diagnostics)
- Scoped
- Singleton
- Dependency Injection
- Custom Model Binder
- Custom Data Annotation
- File Upload
- Insomnia (API Client)
- SQL Server
- ADO.NET (LINQ to Entities)
- ASP.NET Core
- .NET Core
- Password-Hashing (Bcrypt)

## Getting Started

### Database

This database is developed in SQL Server. To interact with the database in a more friendly way, Entity Framework is used to map the database into C# classes. To make a seamless relation, `LINQ to Entities` plays as a bridge between the API and the database for a fluent data transferring.

#### 1. Database Diagram

Given that a Library is being simulated.  The relationships were modeled as faithful as possible to the real life, respecting the relational mapping.

Assuming a Library can store three copies of each book, the quantity was limited to that number. Allowing a Book to be borrowed up to three times. The Member (Person) can have only one unique account called, Reader, allowing it to borrow as many books as he wants as long a copy is available. On the other hand we have the Librarian, whose role is not directly related to the tables relationships, neither the EndUser. Reader, and Librarian are an unique EndUser, it's just a separation of concerns, otherwise, Reader, and Librarian would have been on the same table, which can't be, due to the design. Only a Reader can borrow books not a Librarian.

![Database Diagram](E:\Programming\API\Books Lender_Borrower\Library\Resources\Database Diagram.png)

#### 2. Database Source Code

```sql
USE [LibraryDB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Book](
	[ID] [nchar](60) NOT NULL,
	[Title] [nchar](45) NULL,
	[Author] [nchar](30) NULL,
	[Genre] [nchar](20) NULL,
	[Year] [int] NULL,
	[Editorial] [nchar](20) NULL,
	[Cover] [varbinary](max) NULL,
	[Available] [bit] NULL,
 CONSTRAINT [PK__Book__3DE0C227CDF9E5BA] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Borrow](
	[ID] [nchar](30) NOT NULL,
	[BorrowDate] [datetime] NULL,
	[DueDate] [datetime] NULL,
	[ReturnDate] [datetime] NULL,
	[Reader] [nchar](20) NOT NULL,
	[Book] [nchar](60) NOT NULL,
 CONSTRAINT [PK__Borrow__4295F85FF88232C2] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EndUser](
	[ID] [nchar](20) NOT NULL,
	[Username] [nchar](12) NULL,
	[Password] [nvarchar](max) NULL,
 CONSTRAINT [PK__EndUser__E18F2163B9574016] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Librarian](
	[ID] [nchar](20) NOT NULL,
	[EndUser] [nchar](20) NOT NULL,
 CONSTRAINT [PK__Libraria__E4D86D9D9BE67E07] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ__Libraria__E18F21624C2B349E] UNIQUE NONCLUSTERED 
(
	[EndUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Member](
	[ID] [nchar](12) NOT NULL,
	[Name] [nchar](35) NULL,
	[Phone] [nchar](9) NULL,
	[Email] [nchar](25) NULL,
	[Age] [int] NULL,
 CONSTRAINT [PK__Member__0CF04B382F483CE5] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reader](
	[ID] [nchar](20) NOT NULL,
	[Member] [nchar](12) NOT NULL,
	[EndUser] [nchar](20) NOT NULL,
 CONSTRAINT [PK__Reader__8E67A5815D41B4EC] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ__Reader__0CF04B397504B80A] UNIQUE NONCLUSTERED 
(
	[Member] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ__Reader__E18F2162F1FFB626] UNIQUE NONCLUSTERED 
(
	[EndUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Borrow]  WITH CHECK ADD  CONSTRAINT [FK__Borrow__BookID__47DBAE45] FOREIGN KEY([Book])
REFERENCES [dbo].[Book] ([ID])
GO
ALTER TABLE [dbo].[Borrow] CHECK CONSTRAINT [FK__Borrow__BookID__47DBAE45]
GO
ALTER TABLE [dbo].[Borrow]  WITH CHECK ADD  CONSTRAINT [FK__Borrow__ReaderID__46E78A0C] FOREIGN KEY([Reader])
REFERENCES [dbo].[Reader] ([ID])
GO
ALTER TABLE [dbo].[Borrow] CHECK CONSTRAINT [FK__Borrow__ReaderID__46E78A0C]
GO
ALTER TABLE [dbo].[Librarian]  WITH CHECK ADD  CONSTRAINT [FK__Librarian__EndUs__440B1D61] FOREIGN KEY([EndUser])
REFERENCES [dbo].[EndUser] ([ID])
GO
ALTER TABLE [dbo].[Librarian] CHECK CONSTRAINT [FK__Librarian__EndUs__440B1D61]
GO
ALTER TABLE [dbo].[Reader]  WITH CHECK ADD  CONSTRAINT [FK__Reader__EndUserI__403A8C7D] FOREIGN KEY([EndUser])
REFERENCES [dbo].[EndUser] ([ID])
GO
ALTER TABLE [dbo].[Reader] CHECK CONSTRAINT [FK__Reader__EndUserI__403A8C7D]
GO
ALTER TABLE [dbo].[Reader]  WITH CHECK ADD  CONSTRAINT [FK__Reader__MemberID__3F466844] FOREIGN KEY([Member])
REFERENCES [dbo].[Member] ([ID])
GO
ALTER TABLE [dbo].[Reader] CHECK CONSTRAINT [FK__Reader__MemberID__3F466844]
GO
```

#### 3. Scaffold-DbContext Generation

To be able to inject queries against the database, i.e., the Entity Framework, a `Database Context` must be created. In this case the Context would be the database, but as C# classes.

The process is very simple, just by executing the down below command line on the IDE terminal we will generate the Scaffold-DbContext.

`Scaffold-DbContext "Server=[Hostname];Database=[Database Name];Trusted_Connection=True;TrustServerCertificate=True;"Microsoft.EntityFrameworkCore.SqlServer -OutputDir ..\DAL\Models`

### Dependencies

To add most of its features, dependencies must be installed from the NuGet Package Manager.

1. **BCrypt**: it allows to hash the user's password.
2. **JSON Model Binder**: it allows to bind JSON serialized parts in a multipart/form-data request for the controller actions.
3. **Authentication JWT Bearer**: it enables support for JWT (JSON Web Token) based authentication. It allows to authenticate users by validating JWT tokens in the Authorization header of HTTP requests
4. **Authorization**: it provides support for authorization.
5. **Open AI (Swagger)**: it provides support for generating documentation for the API through this built-in API client.
6. **Entity Framework Core**: it allows to interact with the database using objects instead of raw queries.
7. **Entity Framework Core Design**: it allows to generate the scaffolding of the database.
8. **Entity Framework Core SQL Server**: it adds support for working with SQL Server.
9. **Entity Framework Core Tools**: it provides essential tools for the Entity Framework.
10. **Caching Stack Exchange Redis**: it allows the use of Redis cache system.
11. **Serilog ASP.NET Core**: it allows the use of a logging library to perform diagnostics during the program's debugging.
12. **Serilog Sinks Console**: it allows to write the log event to the console.
13. **Serilog Sinks File**: it allows to write the log event to a file.
14. **Swashbuckle ASP.NET Core**: it allows the use of the API client called, 'Swagger'.
15. **Identity Model Tokens JWT**: it allows the creation, and validations of the JWT.

### AppSettings Configuration

It stores configuration settings of the API's features. Where the Serilog, Token, and Redis settings are declared.

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.json",
          "rollingInterval": "Day",
          "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
        }
      }
    ]
  },

  "AllowedHosts": "*",
  "Settings": {
    "SecretKey": "=[RE$74P1T0KEN]="
  },
    
  "ConnectionStrings": {
    "RedisConnection": "127.0.0.1:6379"
  }
}
```

**Serilog:** a console and a file sinks are declared. A sink is an output destination, meaning, that the log events will be displayed on the console as the API is executed, and stored on a specified file location once the execution has ended. There are many sinks, but, only these two were selected, cause, they are more suitable for this case scenario.

**Token**: given that this is a demonstration environment, any host is allowed to perform this token validation. The `SecretKey` is used to secure the token by signing it, this ensures that the token hasn't been tampered during the request. It allows to validate the token integrity, and it should be kept in secret, as it name implies.

**Redis:** the cache system works locally on the host RAM. The host address, and the Redis port are declared.

### Program

It's the entry point for the API. It sets up the necessary configuration and services to run. When the API is executed, `Program.cs` is the first file to initiate. `Program.cs` reads from `Appsettings.json` to get the proper configuration values.

```c#
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new CustomBinderProviderService());
});

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Server=TUF293;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;")));

builder.Services.AddSingleton<IConnectionMultiplexer>(redis =>
ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")));

builder.Services.AddScoped<CacheManagerService>();

builder.Services.AddScoped<CacheService>();

builder.Services.AddScoped<ClaimVerifierService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<HelperService>();

builder.Configuration.AddJsonFile("appsettings.json");
var secretKey = builder.Configuration.GetSection("Settings").GetSection("SecretKey").ToString();
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(config =>
{

    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration).CreateLogger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

**builder.Services.AddControllers... :** a "Custom Model Binder" is needed to handle the incoming image file from the client along with the "Custom Data Annotation".

**builder.Services.AddDbContext\<LibraryDbContext>... :** due to the Context is the database. It needs to be accessible on every request. That's why is declared as Scoped.

**builder.Services.AddSingleton\<IConnectionMultiplexer>... :** cache needs to be always available during the API lifetime, so, it was declared as Singleton.

**builder.Services.AddScoped\<CacheManagerService>()**, **builder.Services.AddScoped\<CacheService>()**, **builder.Services.AddScoped\<ClaimVerifierService>()**, **builder.Services.AddScoped\<HelperService>()**: are Scoped support classes playing the role of SOLID to lighten the overload of some classes.

**builder.Services.AddHttpContextAccessor()**: there is a class called, `ClaimVerifierService` which manages the user's request claim for authorization purposes. The `HTTPContextAccessor` permit to this class to access to the User content from the HTTP.

**builder.Configuration.AddJsonFile... :** this declaration adds the JWT configuration from appsettings.

**builder.Services.AddAuthentication:** more JWT configuration declaration.

**Log.Logger = new LoggerConfiguration():** adds Serilog to the configuration, allowing its logs during execution.

## Business
### Use Cases

![Use Cases](E:\Programming\API\Books Lender_Borrower\Use Cases.png)

A Library system is too big to replicate it on an API just for showcasing, so, it opted to illustrate the main function of it, the borrow, and the user management systems accordingly.

There are only two entities present on this business logic, the Librarian who is in charge of managing the books, the loans, and the readers. On the other hand, the Reader who can search for a book, view a list of books based on a criteria, and borrow them.

### Structure

**Controllers:** every incoming HTTP request is received, and processed by the endpoints inside their respective Controller. Controllers are the core of the API.

**CustomDataAnnotations:** a custom validator was built to handle the incoming image file attributes.

**Logs:** has all the results as text files from the debugging. To keep a track of the generated values.

**Models**: this contains all the tables mapped to a class along with `LibraryDbContext`.

**Services:** all the support classes to produce leverage on the logic.

### Logic

#### 1. Sign Up & Login: `MemberController`

##### Overview

It holds the Sign Up, and Login endpoints mainly. As their names implies, they take care of the user account creation, and authentication. Before any process is granted, the incoming data either for Sign Up or Login must be verified based on a criteria, after the integrity has been correctly proven right, the subsequent process is good to continue.
##### Key Components

- Password Hashing
- Token
- Authentication
- Sign Up

##### Important Methods

The `SignUp()` uses the `Hash()`, and `HasVerifier()` to hash the  user's new password to keep it safe, so no one in the database can guess it. The Hash process borrows the BCrypt Library for its aim. As for the `Login()`, `CreateToken()` performs the JWT creation, generating a `Claim Key` on the process, a key that's comprised of the user's ID . Now on, every time the user requests some information, the endpoint will validate the user's identity before continue on the response, this will ensure that no one that's not authorized on this API perform a requests.

##### Source Code

This method is a slight different from Member, and Librarian Sign Up due to the nature of the user type, but both serve as the main purpose, register the new users to the system. Any of these two were chosen for explanation purposes.

This method handles a POST request cause data is send for creation. It gets a JSON member object to be deserialized into a C#, for the server manipulation. A check is performed against the context for any duplicates, then an error message is returned, or a successful one with the new Member created on the database.

```c#
        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] ReaderService newMember)
        {
            try
            {
                if (newMember == null || !ModelState.IsValid)
                {
                    return BadRequest();
                }

                var memberDAL = Context.Members.FirstOrDefault(member => member.Id == newMember.IDMember);

                if (memberDAL != null)
                {
                    return BadRequest("This user already exists!");
                }

                Context.Members.Add(MappingMember(newMember));

                EndUserReader(newMember);

                await Context.SaveChangesAsync();
                
                return Created("", "Your account has been successfully created.");
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "A database error occurred. Please try again.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

```

Here, the Login() works both for the Reader and the Librarian user. Just like the SignUp(), it gets a JSON user object which is deserialized for its use on the server. After the existence validation, `HasVerifier()` checks if the entered password matches the one stored in the database, and `UsernameComparison()` checks if the username also matches the stored one. After both methods return true, a token is created to use for the entire API lifetime as an authentication inside the request.

```c#
        [HttpPost]
        [Route("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] UserService newUser)
        {
            try
            {
                if (newUser == null || !ModelState.IsValid)
                {
                    return BadRequest();
                }

                var users = Context.EndUsers
                    .Where(user => user.Username == newUser.Username)
                    .ToList();
                
                var userDAL = users.FirstOrDefault(user => HashVerifier(newUser.Password, user.Password)
                                                           && UsernameComparison(newUser.Username, user.Username));

                if (userDAL != null)
                {
                    return Ok(CreateToken(userDAL.Id));
                }

                return NotFound("This user doesn't exist.");
            }
            catch (DbUpdateException ex)
            {
                return BadRequest("A DbUpdateException has occurred: " + ex);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest("An exception has occurred: " + ex);
            }
            catch (Exception ex)
            {
                return BadRequest("An exception has occurred: " + ex);
            }
        }
```



#### 2. Book CRUD: `LibrarianBookController`

##### Overview

As far as we know so far, the Librarian takes care of the administration whether is for the users, and the books. Here, he's able to add a new book, update the book attributes, remove a book, and search for a book. 

A new extremely useful feature is gonna come to play, Redis. It's an in-memory data structure store used as a fast volatile database. Only GET requests are equipped with a cache, which a request response time must be decrease significantly due to the heavy traffic that implies.

##### Key Components

- Claim Validation
- Cache

##### Important Methods

This entire CRUD bases its main functionality in two classes, `ClaimVerifier`, and `CacheManagerService`, are the backbone of every endpoint.

When the Librarian performs any CRUD action. The first procedure that comes into play is the user's claim validation, it has to make sure that the claim has a valid token. Afterwards, along with this method, it must check what type of user is the one who's trying to perform a CRUD. This is by checking the first character of the Claim, which is the ID. If it starts with 'L', is the Librarian, if it starts with a number, is a Reader.

Regarding the use of the cache system. Every value that is requested by the client side, that's not already on the cache, is injected as a new memory space so if the user come to request the same data as before, the response will be way faster than before. When the Librarian updates any value located on any cache, the system automatically will replace the old for the new entry, so the cache is always updated. 

##### Source Code

A book is comprised of a JSON, and an image being its cover. When its creation is requested, both the JSON, and the image file are handled by the `Custom Model Binder`, that ensures both are loaded deserialized properly into C# objects, for its respective `Custom Data Validation`, besides of the book meeting the requirements, the image file also has to be based on the business criteria. After both being validated accordingly the rest of the pipeline can be executed. 

```c#
        [HttpPost]
        [Route("AddBook")]
        [Authorize]
        public async Task<IActionResult> CreateBook([ModelBinder(BinderType = typeof(CustomBinderService))] BookCoverService incomingBook)
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (ClaimVerifier.ClaimID.StartsWith('L'))
                {
                    if (incomingBook == null || !ModelState.IsValid)
                    {
                        return BadRequest();
                    }

                    var bookDAL = await Context.Books.FirstOrDefaultAsync(book => book.Id == incomingBook.ID);

                    if (bookDAL != null)
                    {
                        return BadRequest();
                    }

                    if (HelperService.CheckBookStorage(incomingBook.Title.Trim()) >= 3)
                    {
                        return BadRequest("The library is limited to 3 copies per book.");
                    }

                    Context.Books.Add(MappingBookCover(incomingBook));
                    await Context.SaveChangesAsync();

                    return Created("", $"\"{incomingBook.Title}\" has been added to the Library.");

                }
                if (Char.IsDigit(ClaimVerifier.ClaimID[0]))
                {
                    return Unauthorized("This user has no authorization to perform this action.");
                }

                return BadRequest("Invalid request.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }
```

Most of the endpoints have the same principle. We're gonna focus on this time on the cache management. `CacheManagerService` manipulate all the cache on every request, it's responsible for calling the cache methods, whether is setting a new memory space, getting an existing value, or removing a memory space.

```c#
        [HttpGet("ViewBook/{ID}")]
        [Authorize]
        public async Task<ActionResult<BookService>> DisplayBook([FromRoute] String ID)
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (ClaimVerifier.ClaimID.StartsWith('L'))
                {
                    var isCacheBook = CacheManagerService.CacheService.Get<BookService>($"book:{ID}");

                    if (isCacheBook != null)
                    {
                        return isCacheBook;
                    }

                    var bookDAL = await Context.Books.FirstOrDefaultAsync(book => book.Id == ID);

                    if (bookDAL != null)
                    {
                        CacheManagerService.CacheService.Set(ID, MappingBook(bookDAL));

                        return MappingBook(bookDAL);
                    }

                    return NotFound();
                }
                if (Char.IsDigit(ClaimVerifier.ClaimID[0]))
                {
                    return Unauthorized("This user has no authorization to perform this action.");
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }
```

#### 3. Book Loan: `LibrarianLoanController`

##### Overview

The book loan is another important task of the Librarian. He must check regularly, and keep track of every book loan status. 

On this controller, he is able to do two major things. One, is to list every single borrow with meaningful information made by the readers, and to update the loan status to "returned". 

##### Key Components

- Claim Validation
- Cache

##### Important Methods

On this method call. As every endpoint, it does a claim validation, and a user authentication. If they are any loans, it will return a list of every single one of them, otherwise, it will return a message indication that is no loan available to show given that no reader has borrowed a book so far. Its functioning is not rocket science.

```c#
        [HttpGet]
        [Route("ViewLoans")]
        [Authorize]
        public async Task<ActionResult<List<BorrowInformationService>>> DisplayLoans()
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (ClaimVerifier.ClaimID.StartsWith('L'))
                {
                    var checkLoan = CacheManagerService.CacheService.Get<List<BorrowInformationService>>("book:loans");

                    if (checkLoan != null)
                    {
                        return checkLoan;
                    }

                    var allLoans = await Context.Borrows.ToListAsync();

                    if (allLoans.Any())
                    {
                        var allBooksLoan = MappingAllLoans(allLoans);

                        CacheManagerService.CacheService.Set("loans", allBooksLoan);

                        return allBooksLoan;
                    }

                    return NotFound("Readers haven't requested a book.");
                }
                if (Char.IsDigit(ClaimVerifier.ClaimID[0]))
                {
                    return Unauthorized("This user has no authorization to perform this action.");
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");

            }
        }
```

As for the book loan update. The Librarian, must submit the reader's ID, and the book name that is being returned. With those two entry parameters, this method checks for any matches on the context, if it does find any matches, it will set the current date to the return date attribute of the Borrow (Loan) model, saying, that this book was returned that same day.

```c#
        [HttpPut("UpdateLoan/{bookReturned}, {reader}")]
        [Authorize]
        public async Task<IActionResult> ReturnBook([FromRoute] String bookReturned, [FromRoute] String reader)
        {
            try
            {
                if (!ClaimVerifier.ClaimValidation())
                {
                    return Unauthorized("This user doesn't exist.");
                }

                if (ClaimVerifier.ClaimID.StartsWith('L'))
                {
                    if (CheckLoans() == 0)
                    {
                        return NotFound("Readers haven't requested any book.");
                    }

                    var borrowDAL = await Context.Borrows.FirstOrDefaultAsync(borrow => borrow.Id.Trim() == 
                        BorrowID(CleanedReturnedBook(bookReturned).Trim(),
                        reader + "-Reader").Trim() && borrow.Reader.Trim() == reader + "-Reader".Trim());

                    if (borrowDAL != null)
                    {
                        borrowDAL.ReturnDate = DateTime.Now;

                        AvailableAgain(ReadyBookAvailable(borrowDAL.Id.Trim()));
                        
                        CacheManagerService.IsLoan(MappingLoan(borrowDAL));

                        await Context.SaveChangesAsync();

                        return NoContent();
                    }

                    return Conflict();

                }
                if (Char.IsDigit(ClaimVerifier.ClaimID[0]))
                {
                    return Unauthorized("This user has no authorization to perform this action.");
                }

                return BadRequest("Invalid request.");

            }
            catch (Exception ex)
            {
                return StatusCode(400, "An unexpected error occurred. Please try again.");
            }
        }
```

#### 4. Librarian-Reader: `LibrarianReaderController`

##### Overview

