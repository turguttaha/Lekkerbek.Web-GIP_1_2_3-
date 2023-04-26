using Quartz;
using System.Net.Mail;
using System.Net;
using Lekkerbek.Web.Services;
using Lekkerbek.Web.Data;
using Lekkerbek.Web.Models;

namespace Lekkerbek.Web.Jobs
{
    public class SendEmailJob : IJob
    {
        private readonly LekkerbekContext _context = new LekkerbekContext();
        private readonly IOrderService _orderService;

        public SendEmailJob(IOrderService orderService)
        {
            _orderService = orderService;
        }
        private List<Order> GetOrders()
        {
            if (_orderService == null)
            {
                Console.WriteLine("NOT WORKING");
            } else
            {
                Console.WriteLine("WORKING");
            }
            //_context.Orders.Find(id);
            return _context.Orders.Select(order => new Order
            //This is another way to make a new object
            {
                OrderID = order.OrderID,
                Finished = order.Finished,
                CustomerId = order.CustomerId,
                Discount = order.Discount,
                TimeSlotID = order.TimeSlotID,
                TimeSlot = new TimeSlot()
                {
                    StartTimeSlot = order.TimeSlot.StartTimeSlot,
                    ChefId = order.TimeSlot.ChefId,
                    Id = order.TimeSlot.Id
                },
                Customer = new Customer()
                {
                    CustomerId = order.Customer.CustomerId,
                    FName = order.Customer.FName,
                    LName = order.Customer.LName,
                    BtwNumber = order.Customer.BtwNumber,
                    Btw = order.Customer.Btw,
                    City = order.Customer.City,
                    ContactPerson = order.Customer.ContactPerson,
                    Birthday = order.Customer.Birthday,
                    LoyaltyScore = order.Customer.LoyaltyScore,
                    Email = order.Customer.Email,
                    PhoneNumber = order.Customer.PhoneNumber,
                    PreferredDishId = order.Customer.PreferredDishId,

                }

            }).Where(c => c.Finished == false).ToList();

        }
        public Task Execute(IJobExecutionContext context)
        {
            var list = GetOrders();
            foreach (var item in list)
            {
                DateTime startTimeSlot = item.TimeSlot.StartTimeSlot;
                DateTime endTimeSlot = startTimeSlot.AddMinutes(15);
                DateTime now = DateTime.Now;
                DateTime threeHoursAgo = endTimeSlot.AddMinutes(-180);
                DateTime threeHoursAgoOneMinLater = endTimeSlot.AddMinutes(-179);


                if (threeHoursAgoOneMinLater > now && threeHoursAgo < now  && item.Customer.Email != null)
                {
                    string fromMail = "gipteam2.lekkerbek@gmail.com";
                    string fromPassword = "pagwjgwdlutmgpfj";

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(fromMail);
                    message.Subject = "Your order is almost ready!";
                    message.To.Add(new MailAddress("taha.turgut.1634@gmail.com"));
                    //message.To.Add(new MailAddress(item.Customer.Email));
                    message.Body = " Mr./Mrs." + item.Customer.FName + " " + item.Customer.LName + ",\n\n Your order will be ready at " + endTimeSlot.TimeOfDay + ". Please come to restaurant at exact time to pick up. \n\n Have Nice Day \n Lekkerbek";



                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(fromMail, fromPassword),
                        EnableSsl = true,
                    };

                    smtpClient.Send(message);
                }





            }
            return Task.FromResult(true);

        }
        //public Task Execute(IJobExecutionContext context)
        //{
        //    string fromMail = "gipteam2.lekkerbek@gmail.com";
        //    string fromPassword = "pagwjgwdlutmgpfj";

        //    MailMessage message = new MailMessage();
        //    message.From = new MailAddress(fromMail);
        //    message.Subject = "Your order is almost ready!";
        //    message.To.Add(new MailAddress("taha.turgut.1634@gmail.com"));
        //    //message.Body = " Mr./Mrs." + item.Customer.FName + " " + item.Customer.LName + " Your order will be ready at " + endTimeSlot + ". Please come to restaurant at exact time to pick up. \n Have Nice Day \n Lekkerbek";



        //    var smtpClient = new SmtpClient("smtp.gmail.com")
        //    {
        //        Port = 587,
        //        Credentials = new NetworkCredential(fromMail, fromPassword),
        //        EnableSsl = true,
        //    };

        //    smtpClient.Send(message);
        //    return Task.FromResult(true);
        //}
    }
}
