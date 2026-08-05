# 📅 Weekly Focus - Smart Weekly Task Manager

<p align="center">
  <img src="https://img.shields.io/badge/Status-Live%20on%20Render-brightgreen" alt="Live Status">
  <img src="https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet" alt=".NET 10">
  <img src="https://img.shields.io/badge/Database-SQLite-blue?logo=sqlite" alt="SQLite">
  <img src="https://img.shields.io/badge/Deploy-Docker-2496ED?logo=docker" alt="Docker">
  <img src="https://img.shields.io/badge/License-MIT-green" alt="MIT License">
</p>

> **"It’s not just a to-do list; it’s a mirror reflecting your habits."**

---

## 🌟 The Legend (Why This Exists)

Most task managers just let you tick a box. **Weekly Focus** was built to do more. 

This project was born from a simple desire: **to understand why we fail, not just what we finish.** It features a unique **"Week Mirror"**—a smart analytical dashboard that reveals your procrastination patterns, rewards you with XP for tackling hard tasks, and gives you a motivational nudge to keep going.

**This isn't just code. This is a self-improvement tool forged in C#.**

---

## 🚀 Live Demo

**See it in action:**  
👉 **[https://weeklyfocus.onrender.com](https://weeklyfocus.onrender.com)** 👈

*(Note: The free instance spins down after inactivity. The first request might take ~30 seconds to wake up!)*

---

## ✨ The Powers (Features)

- **🗓️ Weekly Planning:** Create and manage specific weeks to organize your workload.
- **🎯 Smart Tasks:** Add tasks with **Priority** (High/Medium/Low), **Difficulty** (Hard/Medium/Easy), and Expected Time.
- **📊 XP & Gamification:** Earn Experience Points (XP) based on task difficulty and time spent.
- **🪞 The Week Mirror (The Crown Jewel):** Get deep insights at the end of the week:
  - Your most frequent failure reason.
  - The hardest task you actually completed.
  - A personalized motivational message.
- **🎨 Beautiful UI:** Dark/Light mode toggle, Confetti celebrations, Keyboard shortcuts (`N` to add a task).
- **🔒 Zero Setup:** No login required. Uses a smart `AnonymousId` stored in your browser.

---

## 🛠️ The Forge (Tech Stack)

| Layer | Technology |
| :--- | :--- |
| **Backend API** | ASP.NET Core 10 (C#) |
| **Database** | SQLite (Simple, file-based, zero-config) |
| **ORM** | Entity Framework Core |
| **Frontend** | Vanilla HTML5, CSS3, JavaScript |
| **Validation** | FluentValidation |
| **Deployment** | Docker + Render.com (Free Tier) |

---

## 🗂️ The Realm (Project Structure)

```text
WeeklyFocus/
├── TaskManagement/               # The Core Engine
│   ├── Controllers/              # API Endpoints
│   ├── Data/                     # DbContext
│   ├── Dtos/                     # Data Transfer Objects
│   ├── Enums/                    # Priorities, Difficulties, Statuses
│   ├── Helpers/                  # Utility functions
│   ├── Interfaces/               # Contracts for DI
│   ├── Middleware/               # Global Exception Handling
│   ├── Models/                   # Entities (Week, TaskItem, FailureReason)
│   ├── Services/                 # Business Logic (The Brain)
│   ├── Validators/               # FluentValidation rules
│   ├── wwwroot/                  # The Frontend (HTML, CSS, JS)
│   │   ├── index.html            # Main UI
│   │   ├── css/                  # Styles (Dark/Light support)
│   │   └── js/                   # Vanilla JS
│   ├── Dockerfile                # Containerization for Render
│   ├── appsettings.json          # Configuration
│   └── Program.cs                # Application Entry Point
└── TaskManagement.sln            # Solution File
