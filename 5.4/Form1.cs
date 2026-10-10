namespace TreeView_ListView
{
    public partial class Form1 : Form
    {
        // Simple in-memory employee model
        private class Employee
        {
            public string Id { get; set; }
            public string FullName { get; set; }
            public string Position { get; set; }
            public DateTime HireDate { get; set; }
            // Department path used to match TreeNode.Tag (e.g., "Company/Dept/Group")
            public string DeptPath { get; set; }
            public int ImageIndex { get; set; }
        }

        private List<Employee> employees = new List<Employee>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Prepare some icons
            imageList.Images.Add(System.Drawing.SystemIcons.Application.ToBitmap());
            imageList.Images.Add(System.Drawing.SystemIcons.Information.ToBitmap());
            imageList.Images.Add(System.Drawing.SystemIcons.Warning.ToBitmap());

            cbViewMode.SelectedIndex = 0; // Details

            BuildSampleData();
            BuildTree();
            // Select root node
            if (tvDepartments.Nodes.Count > 0)
            {
                tvDepartments.SelectedNode = tvDepartments.Nodes[0];
            }
        }

        private void BuildSampleData()
        {
            // Company -> Departments -> Groups
            employees.Clear();
            employees.Add(new Employee { Id = "E001", FullName = "Nguyễn Văn A", Position = "Trưởng phòng", HireDate = new DateTime(2018, 3, 12), DeptPath = "ACME/HR", ImageIndex = 0 });
            employees.Add(new Employee { Id = "E002", FullName = "Trần Thị B", Position = "Nhân viên", HireDate = new DateTime(2019, 7, 1), DeptPath = "ACME/HR", ImageIndex = 1 });
            employees.Add(new Employee { Id = "E003", FullName = "Lê Văn C", Position = "Nhân viên", HireDate = new DateTime(2020, 1, 10), DeptPath = "ACME/IT/Dev", ImageIndex = 2 });
            employees.Add(new Employee { Id = "E004", FullName = "Phạm Thị D", Position = "Kỹ sư", HireDate = new DateTime(2021, 5, 5), DeptPath = "ACME/IT/QA", ImageIndex = 2 });
            employees.Add(new Employee { Id = "E005", FullName = "Hoàng Văn E", Position = "Nhóm trưởng", HireDate = new DateTime(2017, 9, 20), DeptPath = "ACME/Sales", ImageIndex = 0 });
        }

        private void BuildTree()
        {
            tvDepartments.Nodes.Clear();

            var root = new TreeNode("ACME") { Tag = "ACME", ImageIndex = 0, SelectedImageIndex = 0 };

            var hr = new TreeNode("HR") { Tag = "ACME/HR", ImageIndex = 1, SelectedImageIndex = 1 };
            var it = new TreeNode("IT") { Tag = "ACME/IT", ImageIndex = 2, SelectedImageIndex = 2 };
            var sales = new TreeNode("Sales") { Tag = "ACME/Sales", ImageIndex = 0, SelectedImageIndex = 0 };

            var itDev = new TreeNode("Dev") { Tag = "ACME/IT/Dev", ImageIndex = 2, SelectedImageIndex = 2 };
            var itQa = new TreeNode("QA") { Tag = "ACME/IT/QA", ImageIndex = 2, SelectedImageIndex = 2 };

            it.Nodes.Add(itDev);
            it.Nodes.Add(itQa);

            root.Nodes.Add(hr);
            root.Nodes.Add(it);
            root.Nodes.Add(sales);

            tvDepartments.Nodes.Add(root);
            root.Expand();
        }

        private void tvDepartments_AfterSelect(object sender, TreeViewEventArgs e)
        {
            LoadEmployeesForNode(e.Node);
        }

        private void LoadEmployeesForNode(TreeNode node)
        {
            lsvEmployees.BeginUpdate();
            lsvEmployees.Items.Clear();
            // Ensure details columns exist
            if (lsvEmployees.View == View.Details && lsvEmployees.Columns.Count == 0)
            {
                lsvEmployees.Columns.Add("Mã NV", 80);
                lsvEmployees.Columns.Add("Họ Tên", 180);
                lsvEmployees.Columns.Add("Chức vụ", 120);
                lsvEmployees.Columns.Add("Ngày vào làm", 120);
            }

            string tag = node?.Tag as string;
            if (string.IsNullOrEmpty(tag)) tag = node?.Text;

            var filtered = employees.Where(emp => emp.DeptPath == tag).ToList();
            foreach (var emp in filtered)
            {
                var item = new ListViewItem(emp.Id) { ImageIndex = emp.ImageIndex };
                item.SubItems.Add(emp.FullName);
                item.SubItems.Add(emp.Position);
                item.SubItems.Add(emp.HireDate.ToString("yyyy-MM-dd"));
                lsvEmployees.Items.Add(item);
            }

            // If not details view, show as large icons or list
            if (lsvEmployees.View != View.Details)
            {
                // Make sure items show main text (FullName) instead of Id
                foreach (ListViewItem it in lsvEmployees.Items)
                {
                    var emp = employees.FirstOrDefault(x => x.Id == it.Text);
                    if (emp != null)
                    {
                        it.Text = emp.FullName;
                    }
                }
            }

            lsvEmployees.EndUpdate();
        }

        private void cbViewMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbViewMode.SelectedItem as string)
            {
                case "Details":
                    lsvEmployees.View = View.Details;
                    break;
                case "SmallIcon":
                    lsvEmployees.View = View.SmallIcon;
                    break;
                case "LargeIcon":
                    lsvEmployees.View = View.LargeIcon;
                    break;
                case "Tile":
                    lsvEmployees.View = View.Tile;
                    break;
                case "List":
                    lsvEmployees.View = View.List;
                    break;
            }

            // Refresh current selection to rebuild items properly
            if (tvDepartments.SelectedNode != null)
                LoadEmployeesForNode(tvDepartments.SelectedNode);
        }
    }
}
