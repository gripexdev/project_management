<!-- PROJECT SHIELDS -->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

<!-- PROJECT LOGO -->
<br />
<div align="center">

  <h3 align="center">Project Management</h3>

  <p align="center">
    A web-based dashboard application for managing tasks, projects, and employee workflows.
    <br />
    <a href="https://github.com/gripexdev/project_management"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/gripexdev/project_management">View Demo</a>
    &middot;
    <a href="https://github.com/gripexdev/project_management/issues/new?labels=bug&template=bug-report---.md">Report Bug</a>
    &middot;
    <a href="https://github.com/gripexdev/project_management/issues/new?labels=enhancement&template=feature-request---.md">Request Feature</a>
  </p>
</div>

---

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#key-features">Key Features</a></li>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>

---

<!-- ABOUT THE PROJECT -->
## About The Project


The **ProjectManagement** is a comprehensive web application designed to streamline task management, project tracking, and employee workflows. It provides intuitive tools such as a Kanban board, interactive calendar, and detailed analytics to help teams stay organized and productive.

### Key Features

* **Task Management**: Create, update, and track tasks with drag-and-drop functionality.
* **Kanban Board**: Visualize tasks in a Kanban-style workflow.
* **Interactive Calendar**: View tasks and deadlines on an interactive calendar.
* **User Authentication**: Secure login and registration using ASP.NET Identity.
* **Responsive Design**: Fully responsive UI for desktop and mobile devices.
* **Analytics Dashboard**: Monitor project progress, overdue tasks, and team performance.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Built With

This project leverages the following frameworks, libraries, and tools:

* [![ASP.NET Core][aspnet-core-badge]][aspnet-core-url]
* [![MySQL][mysql-badge]][mysql-url]
* [![FullCalendar][fullcalendar-badge]][fullcalendar-url]
* [![Bootstrap][bootstrap-badge]][bootstrap-url]
* [![Dragula][dragula-badge]][dragula-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

<!-- GETTING STARTED -->
## Getting Started

Follow these steps to set up the **ProjectDashboard** locally or via Docker.

### Prerequisites

Ensure you have the following installed:

* [.NET SDK](https://dotnet.microsoft.com/download) (version 9 or higher)
* [MySQL Server](https://dev.mysql.com/downloads/) (or use Docker for MySQL)
* [Docker](https://www.docker.com/) (optional, for containerized deployment)

### Installation

#### Running Locally

1. Clone the repository:
   ```sh
   git clone https://github.com/gripexdev/project_management.git
   cd project_management
   ```

2. Set up the database:
   - Create a MySQL database named `ProjectDashboardDB`.
   - Update the `ConnectionStrings:DefaultConnection` in `appsettings.json`:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=3306;Database=ProjectDashboardDB;User=root;Password=your_password;"
     }
     ```

3. Apply migrations:
   ```sh
   dotnet ef database update
   ```

4. Run the application:
   ```sh
   dotnet run
   ```

#### Running with Docker

1. Clone the repository:
   ```sh
   git clone https://github.com/gripexdev/project_management.git
   cd project_management
   ```

2. Build and run containers:
   ```sh
   docker-compose up --build
   ```

3. Access the app:
   - Frontend: [http://localhost:5001](http://localhost:5001)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

<!-- USAGE EXAMPLES -->
## Usage

Here are some examples of how you can use the **ProjectDashboard**:

* **Kanban Board**:
  - Drag and drop tasks between columns (Backlog, To Do, In Progress, Done).
  - Update task details such as priority, due date, and progress.

* **Interactive Calendar**:
  - View all tasks and deadlines in a calendar view.
  - Click on a task to view its details.

* **Analytics Dashboard**:
  - Monitor key metrics such as completed tasks, overdue tasks, and project progress.
  - Use the cards to get a quick overview of your team's performance.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

<!-- CONTACT -->
## Contact

Sadiky Othmane - [@GitHub](https://github.com/sadikyothmane)  
EL KHOTRI Omar - elkhotriomarpro@gmail.com  

Project Link: [https://github.com/gripexdev/project_management](https://github.com/gripexdev/project_management)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---


<!-- MARKDOWN LINKS & IMAGES -->
[contributors-shield]: https://img.shields.io/github/contributors/gripexdev/project_management.svg?style=for-the-badge
[contributors-url]: https://github.com/gripexdev/project_management/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/gripexdev/project_management.svg?style=for-the-badge
[forks-url]: https://github.com/gripexdev/project_management/network/members
[stars-shield]: https://img.shields.io/github/stars/gripexdev/project_management.svg?style=for-the-badge
[stars-url]: https://github.com/gripexdev/project_management/stargazers
[issues-shield]: https://img.shields.io/github/issues/gripexdev/project_management.svg?style=for-the-badge
[issues-url]: https://github.com/gripexdev/project_management/issues
[license-shield]: https://img.shields.io/github/license/gripexdev/project_management.svg?style=for-the-badge
[license-url]: https://github.com/gripexdev/project_management/blob/master/LICENSE.txt
[product-screenshot]: images/screenshot.png
[aspnet-core-badge]: https://img.shields.io/badge/ASP.NET%20Core-5C2D91?style=for-the-badge&logo=asp.net&logoColor=white
[aspnet-core-url]: https://dotnet.microsoft.com/apps/aspnet
[mysql-badge]: https://img.shields.io/badge/MySQL-00758F?style=for-the-badge&logo=mysql&logoColor=white
[mysql-url]: https://www.mysql.com/
[fullcalendar-badge]: https://img.shields.io/badge/FullCalendar-378006?style=for-the-badge&logo=fullcalendar&logoColor=white
[fullcalendar-url]: https://fullcalendar.io/
[bootstrap-badge]: https://img.shields.io/badge/Bootstrap-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white
[bootstrap-url]: https://getbootstrap.com/
[dragula-badge]: https://img.shields.io/badge/Dragula-FF6F61?style=for-the-badge&logo=dragula&logoColor=white
[dragula-url]: https://github.com/bevacqua/dragula

---
