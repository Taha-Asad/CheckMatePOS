# CheckMate POS

A console-based Point of Sale (POS) system built with .NET 9 and SQLite, designed for retail environments with role-based access for administrators and cashiers.

## Overview

CheckMate POS provides a complete retail management workflow: product catalog management, customer relationship tracking, shopping cart operations, multi-method payment processing, invoice generation, and business reporting -- all through an interactive text-based interface.

## Tech Stack

| Component         | Technology                          |
|-------------------|-------------------------------------|
| Runtime           | .NET 9.0                            |
| Language          | C# (Nullable, Implicit Usings)      |
| Database          | SQLite via `Microsoft.Data.Sqlite`  |
| Architecture      | 3-Layer (DAL / BLL / UI)            |
| Design Patterns   | Repository, Strategy, Service Layer |

## Features

### Administration
- Product management (CRUD) with stock tracking
- Customer management (CRUD) with spending history
- Cashier account creation and activation/deactivation
- Invoice viewing and detailed lookup
- Sales, customer, product, and inventory reports

### Cashier Operations
- Product browsing and search
- Shopping cart with add/update/remove
- Multi-method checkout (Cash, Credit Card, PayPal)
- Customer association at checkout (optional)
- Invoice generation and past invoice viewing

### Payment Processing
- **Cash** -- amount validation with change calculation
- **Credit Card** -- card number (16 digits), expiration (MM/YY), CVV (3 digits) validation with masked display
- **PayPal** -- email format validation

### Reporting
- Sales report (totals, tax, payment method breakdown)
- Customer spending report (ranked by total spent)
- Product sales report (quantity sold and revenue per product)
- Inventory status report (low stock and out-of-stock alerts)

## Architecture

```
CheckMatePOS/
├── Program.cs                  # Entry point and dependency wiring
├── Models/                     # Data models (User, Product, Customer, Invoice, Payment, CartItem)
├── Interfaces/                 # Repository and gateway abstractions
├── DAL/                        # Data Access Layer (SQLite operations)
│   └── DatabaseHelper.cs       # Connection management, schema init, password hashing
├── BLL/                        # Business Logic Layer
│   ├── AuthService.cs          # Authentication and user management
│   ├── ProductService.cs       # Product CRUD and stock operations
│   ├── CustomerService.cs      # Customer CRUD
│   ├── ShoppingCartService.cs  # Cart operations and checkout flow
│   ├── SalesService.cs         # Invoice retrieval
│   ├── ReportingService.cs     # Report generation
│   ├── PaymentService.cs       # Payment orchestration
│   ├── CreditCardPaymentGateway.cs  # Credit card validation and processing
│   └── PayPalPaymentGateway.cs      # PayPal validation and processing
└── UI/                         # User Interface Layer (console menus)
    ├── LoginScreen.cs          # Login with masked password input
    ├── AdminMenu.cs            # Admin dashboard (17 options)
    └── CashierMenu.cs          # Cashier terminal (10 options)
```

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later

### Build and Run

```bash
# Clone the repository
git clone https://github.com/Taha-Asad/CheckMatePOS.git
cd CheckMatePOS

# Build
dotnet build

# Run
dotnet run
```

The SQLite database (`CheckMatePOS.db`) is created automatically on first run with a default admin account.

### Default Credentials

| Role   | Username | Password   |
|--------|----------|------------|
| Admin  | admin    | admin123   |

## Database Schema

| Table          | Key Fields                                                        |
|----------------|-------------------------------------------------------------------|
| `Users`        | UserId, Username, PasswordHash (SHA-256), Role, IsActive          |
| `Products`     | ProductId, ProductName, Price, QuantityInStock                    |
| `Customers`    | CustomerId, Name, Email, Phone, TotalSpent                       |
| `Invoices`     | InvoiceId, InvoiceNumber, Date, CashierId, CustomerId, Total     |
| `InvoiceItems` | InvoiceItemId, InvoiceId, ProductId, UnitPrice, Quantity         |

## Project Structure

- **Models** -- Plain data classes representing domain entities
- **Interfaces** -- Abstractions for repositories and payment gateways, enabling testability and loose coupling
- **DAL** -- Implements data access using raw SQL with parameterized queries against SQLite
- **BLL** -- Contains business rules, validation logic, and service orchestration
- **UI** -- Console-based menus handling user input/output for each role

## License

This project was developed as part of the Software Quality Assurance and Quality Control course.
