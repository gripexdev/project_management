using System.Threading.Tasks; // Resolve ambiguity with Task
using Microsoft.EntityFrameworkCore; // Required for ToListAsync
using ProjectDashboard.Data;
using ProjectDashboard.Models;

namespace ProjectDashboard.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        // Create a new notification
        public async System.Threading.Tasks.Task CreateNotificationAsync(int employeeId, string message)
        {
            var notification = new Notification
            {
                EmployeeId = employeeId,
                Message = message
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(); // Await database operation
        }

        // Get unread notifications for an employee
        public async System.Threading.Tasks.Task<List<Notification>> GetUnreadNotificationsAsync(int employeeId)
        {
            return await _context.Notifications
                .Where(n => n.EmployeeId == employeeId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(); // Use ToListAsync for asynchronous queries
        }

        // Mark a notification as read
        public async System.Threading.Tasks.Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync(); // Await database operation
            }
        }
    }
}