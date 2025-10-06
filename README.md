# Gabrieland — Event Center Management Platform

## Overview

**Gabrieland** is a modern **event center management system** built with **Blazor WebAssembly** and **.NET 8**, designed to simplify event planning, reservations, and payment processing.  
It connects to an **Oracle SQL database** for secure data storage and provides both a **frontend client** and a **backend API** for efficient and scalable operation.

---

## Architecture

Gabrieland follows a **client-server architecture**:
- **Frontend:** Built with **Blazor WebAssembly**, running entirely in the browser.
- **Backend API:** Developed with **ASP.NET Core Web API**, handling business logic, database communication, and integrations.
- **Database:** Powered by **Oracle SQL**, accessed through secure managed connections.
- **Third-party Integration:** Includes **Stripe** for payment processing and **Blazored** components for UI enhancements.

---

## Features

### 🎯 Core Functionality
- **Event Reservations:** Users can browse available venues and make online reservations.
- **Payment Processing:** Integrated with **Stripe** for secure online payments.
- **Authentication & Authorization:** Custom authentication state provider with token-based user sessions.
- **Email Notifications:** Automatic email confirmations and updates.
- **File Storage:** Supports file uploads (e.g., invoices, event details, or images).
- **Responsive UI:** Built with Blazor for a seamless, app-like experience in the browser.

---

## Technologies Used

### Frontend (Blazor WebAssembly)
- **Blazor WebAssembly**
- **C# and Razor Components**
- **Blazored.Toast** — for user notifications
- **Blazored.LocalStorage** — for secure client-side storage
- **Custom Authentication Provider**
- **HTTP Client** configured to connect with the backend API

### Backend (ASP.NET Core Web API)
- **.NET 8 Web API**
- **Oracle.ManagedDataAccess.Client** — Oracle database integration
- **Swagger/OpenAPI** — API documentation and testing
- **CORS Configuration** — enabled for development across multiple origins
- **Stripe SDK** — payment management
- **Dependency Injection** for modular, testable architecture

### Database
- **Oracle SQL Database**
- Data access through `DataBaseConnector` and service layers like:
  - `ServicioReservaData`
  - `ReservasData`
  - `TiposPagoData`
  - `FacturaData`
