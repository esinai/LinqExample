using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinqExample
{
    public partial class mainWin : Form
    {
        dbEntities db = new dbEntities();
        List<UserTBL> userList;
        List<ConnectionsTBL> conList;
        int pos = 0;
        int pageSize= 123;
        public mainWin()
        {
            InitializeComponent();
        }
       
        private void mainWin_Load(object sender, EventArgs e)
        {
            userList = (from s in db.UserTBL select s).ToList();
            conList = (from s in db.ConnectionsTBL select s).ToList();
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            var tmp = (from s in userList
                       join d in conList
                       on s.id equals d.userID
                       select new
                       {
                           userID = s.id,
                           FullName = s.first_name + ' ' + s.last_name,
                           s.gender,
                           d.loginDateTime,
                           d.logoutDateTime
                       }).ToList();

            var result = tmp.GroupBy(x => new { x.loginDateTime.Value.Year , x.loginDateTime.Value.Month}).ToList();
            List<yearMonthView> yList = new List<yearMonthView>();
            foreach (var item in result)
            {
                yearMonthView y = new yearMonthView();
                y.year = item.Key.Year;
                y.month = item.Key.Month;
                y.count = item.Count();
                yList.Add(y);
            }
            DateTime t1 = DateTime.Now;
            DateTime t2 = DateTime.Now.AddMinutes(-17);
            TimeSpan ts = t1 - t2;
            yList = yList.OrderByDescending(x => x.year).ThenByDescending(x => x.month).ToList();
            dgvShow.DataSource = yList;
        }

        public void showResult()
        {
            var tmp = userList.Skip(pos).Take(pageSize).ToList();

            dgvShow.DataSource = tmp;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (pos - pageSize < 0)
                return;
            pos = pos - pageSize;
            showResult();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (userList.Count - pos < pageSize)
                return;
            pos += pageSize;
            showResult();
        }

        public class yearMonthView
        {
            public int year { get; set; }
            public int month { get; set; }
            public int count { get; set; }
        }

        
    }
}
