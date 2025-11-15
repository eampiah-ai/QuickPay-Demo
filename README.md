Here is a clear, professional **PRODUCT SPEC** you can paste directly into your repo under
`/docs/PRODUCT_SPEC.md` or `README.md`.

It’s concise enough for a portfolio but complete enough to guide development.

---

# **QuickPay – Product Specification**

## **1. Overview**

QuickPay is a lightweight invoicing and payment platform designed for small businesses that need a fast, simple way to send invoices and collect payments online.
It provides:

* A clean dashboard showing business health
* Easy invoice creation and management
* Secure online payment via Stripe Checkout
* Automated status updates via Stripe webhooks
* Email notifications for invoices and receipts

This project serves as a demonstration of rapid full-stack delivery, featuring React + TypeScript (UI), ASP.NET Core (API), Stripe (payments), and Azure (deployment).

---

## **2. Users & Roles**

### **Admin (Business Owner)**

* Logs into the admin panel
* Creates, edits, and manages invoices
* Manages customers
* Views revenue and unpaid balances
* Sends invoices to customers

### **Customer**

* Receives invoice email
* Views invoice details
* Pays securely via Stripe
* Receives payment receipt

Single-tenant, single-admin configuration for demo purposes.

---

## **3. Core User Flows**

### **3.1 Admin Login**

* Admin enters email + password
* API returns JWT token
* Token used to access all admin endpoints

### **3.2 View Dashboard**

* Dashboard shows:

  * Total revenue (last 30 days)
  * Unpaid invoice total
  * Invoices sent this month
  * Revenue chart (last 30 days)

### **3.3 Manage Invoices**

* Admin can:

  * Create invoice
  * Edit invoice
  * Delete invoice
  * Mark invoice as “Sent” → triggers email
  * View invoice status (Draft, Sent, Paid, Overdue)

### **3.4 Manage Customers**

* Admin can:

  * Create customer
  * Edit customer
  * View list of customers

### **3.5 Customer Pays Invoice**

* Customer receives an email with unique invoice link
* Customer opens page, views details
* Customer clicks **Pay**
* Frontend requests a Stripe Checkout Session URL
* Customer pays using Stripe test card
* Stripe redirects back to confirmation page

### **3.6 Automatic Payment Update (Webhook)**

* Stripe sends webhook event → `/payments/webhook`
* System verifies signature
* On `checkout.session.completed`:

  * Create Payment record
  * Mark Invoice as **Paid**
  * Trigger receipt email

---

## **4. System Features**

### **4.1 Authentication**

* Basic Admin login using JWT
* Routes protected with authorization middleware

### **4.2 Invoice Module**

Each invoice includes:

* Customer (reference)
* Amount
* Status
* Description (optional)
* Due date
* Created timestamp

Statuses:

* **Draft**
* **Sent**
* **Paid**
* **Overdue** (calculated automatically)

### **4.3 Payment Integration**

* Stripe Checkout Session
* Metadata includes `invoiceId`
* Stripe Webhooks handle status updates
* All monetary values stored in **integer cents**

### **4.4 Email Notifications**

Use SendGrid or SMTP for:

* Invoice sent
* Payment receipt

### **4.5 Dashboard Analytics**

* Revenue last 30 days
* Unpaid total
* Count of invoices by status
* Revenue chart

---

## **5. API Endpoints (High-Level)**

### **Auth**

* `POST /auth/login`
  Returns JWT for admin.

### **Dashboard**

* `GET /stats/summary`
  Returns revenue, unpaid total, invoices-by-status.

### **Invoices**

* `GET /invoices` (filters: status, search, pagination)
* `POST /invoices`
* `GET /invoices/{id}`
* `PUT /invoices/{id}`
* `DELETE /invoices/{id}`

### **Customers**

* `GET /customers`
* `POST /customers`
* `PUT /customers/{id}`

### **Payments**

* `POST /payments/create-checkout-session`
* `POST /payments/webhook`

### **Public Invoice Pages**

* `GET /public/invoices/{publicId}`
  Returns invoice details without authentication.

---

## **6. Data Model (Entities)**

### **Customer**

| Field     | Type     |
| --------- | -------- |
| Id        | GUID/int |
| Name      | string   |
| Email     | string   |
| CreatedAt | DateTime |

### **Invoice**

| Field       | Type                   |
| ----------- | ---------------------- |
| Id          | GUID/int               |
| CustomerId  | ref                    |
| AmountCents | int                    |
| Status      | enum                   |
| Description | string                 |
| DueDate     | Date                   |
| CreatedAt   | DateTime               |
| PublicId    | GUID (for public link) |

### **Payment**

| Field           | Type     |
| --------------- | -------- |
| Id              | GUID/int |
| InvoiceId       | ref      |
| AmountCents     | int      |
| StripeSessionId | string   |
| CreatedAt       | DateTime |

---

## **7. Tech Stack**

### **Frontend**

* React + TypeScript
* Vite
* ShadCN UI + Tailwind CSS
* Axios (API)
* Chart.js / Recharts

### **Backend**

* ASP.NET Core 8 Minimal API
* EF Core + SQLite / Azure SQL
* Authentication via JWT
* Stripe.NET SDK
* SendGrid/SMTP

### **Infrastructure**

* Azure App Service (API)
* Azure SQL (or SQLite for demo)
* Azure Static Web Apps (frontend)
* GitHub Actions for CI/CD

---

## **8. Non-Functional Requirements**

### **Security**

* JWT auth for admin endpoints
* Stripe webhook signature validation
* HTTPS enforced

### **Performance**

* Invoices and dashboard endpoints should respond <200ms locally
* DB queries optimized through EF Core indexes

### **Maintainability**

* Modular service layer for Payments, Email, Auth
* Clear folder structure: `Controllers`, `Services`, `Repositories`, `Entities`

### **Demo Ready**

* Seed script to populate customers + sample invoices
* Demo link for invoice payment flow
* Recorded 2–3 minute walkthrough video

---

## **9. Roadmap (Optional Stretch Features)**

* Multi-currency support
* PDF invoice export
* Tax & line items
* Multi-tenant accounts
* Customer portal
* Recurring invoices

---