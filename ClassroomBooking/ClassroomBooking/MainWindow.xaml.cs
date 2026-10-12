using ClassroomBooking.Models;
using ClassroomBooking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClassroomBooking
{
    public partial class MainWindow : Window
    {
        private readonly UserService userService = new UserService();
        private readonly RoomService roomService = new RoomService();
        private readonly BookingService bookingService = new BookingService();

        public MainWindow()
        {
            InitializeComponent();

            UserComboBox.ItemsSource = userService.GetUsers();
            RoomComboBox.ItemsSource = roomService.GetRooms();
            BookingDatePicker.SelectedDate = DateTime.Today;

            RefreshBookings();
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(UserComboBox.SelectedItem is User user))
            {
                StatusTextBox.Text = "Выберите пользователя.";
                return;
            }

            if (!(RoomComboBox.SelectedItem is Room room))
            {
                StatusTextBox.Text = "Выберите аудиторию.";
                return;
            }

            if (!BookingDatePicker.SelectedDate.HasValue)
            {
                StatusTextBox.Text = "Выберите дату.";
                return;
            }

            if (!TimeSpan.TryParse(StartTimeTextBox.Text, out TimeSpan start) ||
                !TimeSpan.TryParse(EndTimeTextBox.Text, out TimeSpan end))
            {
                StatusTextBox.Text = "Некорректное время.";
                return;
            }

            if (start >= end)
            {
                StatusTextBox.Text = "Время начала должно быть меньше времени окончания.";
                return;
            }

            var booking = new Booking
            {
                UserId = user.Id,
                RoomId = room.Id,
                Date = BookingDatePicker.SelectedDate.Value,
                StartTime = start,
                EndTime = end
            };

            if (!bookingService.AddBooking(booking))
            {
                StatusTextBox.Text = "Конфликт: аудитория уже занята на это время.";
                return;
            }

            StatusTextBox.Text = "Бронирование создано.";
            RefreshBookings();
        }

        private void RefreshBookings()
        {
            var rooms = roomService.GetRooms();
            var users = userService.GetUsers();

            var items = bookingService.GetBookings()
                .Select(x =>
                {
                    var user = users.FirstOrDefault(u => u.Id == x.UserId);
                    var room = rooms.FirstOrDefault(r => r.Id == x.RoomId);

                    return $"{x.Date:dd.MM.yyyy} {x.StartTime:hh\\:mm}-{x.EndTime:hh\\:mm} | " +
                           $"Пользователь: {user?.Name ?? "—"} | Аудитория: {room?.Number ?? "—"}";
                })
                .ToList();

            BookingsListBox.ItemsSource = items;
        }
    }
}