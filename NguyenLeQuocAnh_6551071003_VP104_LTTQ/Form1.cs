using System;
using System.Data;
using System.Data.SqlClient; 
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace NguyenLeQuocAnh_6551071003_VP104_LTTQ
{
    public partial class Form1 : Form
    {
        private DatabaseConnect db = new DatabaseConnect();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            InitButtonIcons();

            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            btnThem.Enabled = true;
            btnThoat.Enabled = true;

            txtLuongCB.KeyPress += txtLuongCB_KeyPress;
            txtLuongCB.TextChanged += txtLuongCB_TextChanged;
            dgvNhanVien.CellFormatting += dgvNhanVien_CellFormatting;

            LoadData();
        }

        
        private void InitButtonIcons()
        {
            btnThem.Image = MakeIcon(DrawAdd);
            btnThoat.Image = MakeIcon(DrawExit);
            btnSua.Image = MakeIcon(DrawEdit);
            btnXoa.Image = MakeIcon(DrawDelete);
        }

        private static Bitmap MakeIcon(Action<Graphics> draw)
        {
            Bitmap bmp = new Bitmap(24, 24);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                draw(g);
            }
            return bmp;
        }

        private static void DrawAdd(Graphics g)
        {
            using (Pen p = new Pen(Color.FromArgb(34, 139, 34), 4))
            {
                g.DrawLine(p, 12, 4, 12, 20);
                g.DrawLine(p, 4, 12, 20, 12);
            }
        }

        private static void DrawEdit(Graphics g)
        {
            using (Pen p = new Pen(Color.FromArgb(0, 90, 190), 5))
                g.DrawLine(p, 8, 16, 18, 6);
            using (Brush b = new SolidBrush(Color.FromArgb(40, 40, 40)))
                g.FillPolygon(b, new[] { new Point(3, 21), new Point(5, 14), new Point(10, 19) });
        }

        private static void DrawDelete(Graphics g)
        {
            using (Brush b = new SolidBrush(Color.FromArgb(190, 30, 30)))
            {
                g.FillRectangle(b, 9, 2, 6, 2);
                g.FillRectangle(b, 4, 5, 16, 3);
                g.FillRectangle(b, 6, 9, 12, 13);
            }
            using (Pen p = new Pen(Color.White, 1.5f))
            {
                g.DrawLine(p, 10, 11, 10, 20);
                g.DrawLine(p, 14, 11, 14, 20);
            }
        }

        private static void DrawExit(Graphics g)
        {
            using (Pen p = new Pen(Color.FromArgb(150, 20, 20), 2.5f))
            {
                g.DrawRectangle(p, 3, 3, 10, 18);
                g.DrawLine(p, 9, 12, 21, 12);
                g.DrawLine(p, 17, 8, 21, 12);
                g.DrawLine(p, 17, 16, 21, 12);
            }
        }

        
        private void LoadData()
        {
            try
            {
                string sql = "SELECT MaNhanVien AS [Mã Nhân Viên], TenNhanVien AS [Tên Nhân Viên], LuongCoBan AS [Lương Cơ Bản] FROM NhanVien";
                DataTable dt = db.GetData(sql);

                dgvNhanVien.DataSource = dt;

               
                if (dgvNhanVien.Columns.Count > 2)
                {
                    dgvNhanVien.Columns[2].DefaultCellStyle.Format = "N0";
                    dgvNhanVien.Columns[2].DefaultCellStyle.FormatProvider = CultureInfo.InvariantCulture;
                }

                
                dgvNhanVien.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message, "Lỗi Kết Nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtLuongCB_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        
        private bool dangDinhDangLuong = false;

        private void txtLuongCB_TextChanged(object sender, EventArgs e)
        {
            if (dangDinhDangLuong) return;
            dangDinhDangLuong = true;
            try
            {
                string text = txtLuongCB.Text;
                int caret = txtLuongCB.SelectionStart;

                
                int digitsBeforeCaret = 0;
                for (int i = 0; i < caret && i < text.Length; i++)
                    if (char.IsDigit(text[i])) digitsBeforeCaret++;

                string digits = "";
                foreach (char c in text)
                    if (char.IsDigit(c)) digits += c;

                string formatted = "";
                if (digits.Length > 0)
                {
                    digits = digits.TrimStart('0');
                    if (digits.Length == 0) digits = "0";
                    if (digits.Length > 15) digits = digits.Substring(0, 15);
                    formatted = decimal.Parse(digits, CultureInfo.InvariantCulture).ToString("N0", CultureInfo.InvariantCulture);
                }

                if (formatted != text)
                {
                    txtLuongCB.Text = formatted;

                    int pos = 0, count = 0;
                    while (pos < formatted.Length && count < digitsBeforeCaret)
                    {
                        if (char.IsDigit(formatted[pos])) count++;
                        pos++;
                    }
                    txtLuongCB.SelectionStart = pos;
                }
            }
            finally
            {
                dangDinhDangLuong = false;
            }
        }

       
        private bool TryParseLuong(string text, out decimal luong)
        {
            string raw = text.Replace(",", "").Trim();
            return decimal.TryParse(raw, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out luong) && luong >= 0;
        }

        
        private bool TryGetLuong(object value, out decimal luong)
        {
            luong = 0;
            if (value == null || value == DBNull.Value) return false;
            if (value is decimal d) { luong = d; return true; }
            if (value is IConvertible && !(value is string))
            {
                luong = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                return true;
            }
            return TryParseLuong(Convert.ToString(value), out luong);
        }

     
        private void dgvNhanVien_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 2) return;

            if (TryGetLuong(e.Value, out decimal luong))
            {
                e.Value = luong.ToString("N0", CultureInfo.InvariantCulture);
                e.FormattingApplied = true;
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn có muốn thoát không", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private bool KiemTraTrungMa(string maNV)
        {
            string sqlCheck = "SELECT COUNT(*) FROM NhanVien WHERE MaNhanVien = @Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", maNV) };

            int count = Convert.ToInt32(db.ExecuteScalar(sqlCheck, parameters));
            return count > 0;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string maNV = txtMaNV.Text.Trim();
            string tenNV = txtTenNV.Text.Trim();
            string luongCB = txtLuongCB.Text.Trim();

            if (string.IsNullOrEmpty(maNV) || string.IsNullOrEmpty(tenNV) || string.IsNullOrEmpty(luongCB))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!TryParseLuong(luongCB, out decimal luong))
            {
                MessageBox.Show("Lương cơ bản phải là số hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLuongCB.Focus();
                txtLuongCB.SelectAll();
                return;
            }

            if (KiemTraTrungMa(maNV))
            {
                MessageBox.Show("Mã nhân viên bị trùng! Vui lòng nhập mã khác.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaNV.Focus();
                txtMaNV.SelectAll();
                return;
            }

            string sqlInsert = "INSERT INTO NhanVien (MaNhanVien, TenNhanVien, LuongCoBan) VALUES (@Ma, @Ten, @Luong)";
            SqlParameter[] parameters = {
                new SqlParameter("@Ma", maNV),
                new SqlParameter("@Ten", tenNV),
                new SqlParameter("@Luong", luong)
            };

            int rows = db.ExecuteNonQuery(sqlInsert, parameters);
            if (rows > 0)
            {
                MessageBox.Show("Thêm nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ResetStateAfterAction();   
            }
        }

        
        private void dgvNhanVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;                     

            DataGridViewRow row = dgvNhanVien.Rows[e.RowIndex];
            if (row.IsNewRow) return;                        

            txtMaNV.Text = row.Cells[0].Value?.ToString();
            txtTenNV.Text = row.Cells[1].Value?.ToString();

            txtLuongCB.Text = TryGetLuong(row.Cells[2].Value, out decimal luongDaChon)
                ? luongDaChon.ToString("N0", CultureInfo.InvariantCulture)
                : "";

            txtMaNV.ReadOnly = true;

            btnSua.Enabled = true;
            btnXoa.Enabled = true;
            btnThem.Enabled = true;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            string maNV = txtMaNV.Text.Trim();

            if (string.IsNullOrEmpty(maNV))
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dr = MessageBox.Show("Bạn có muốn xóa nhân viên này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                string sqlDelete = "DELETE FROM NhanVien WHERE MaNhanVien = @Ma";
                SqlParameter[] parameters = { new SqlParameter("@Ma", maNV) };

                db.ExecuteNonQuery(sqlDelete, parameters);
                MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }

            ResetStateAfterAction();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maNV = txtMaNV.Text.Trim();
            string tenNV = txtTenNV.Text.Trim();
            string luongCB = txtLuongCB.Text.Trim();

            if (string.IsNullOrEmpty(maNV))
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tenNV) || string.IsNullOrEmpty(luongCB))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên và lương cơ bản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!TryParseLuong(luongCB, out decimal luong))
            {
                MessageBox.Show("Lương cơ bản phải là số hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLuongCB.Focus();
                txtLuongCB.SelectAll();
                return;
            }

            string sqlUpdate = "UPDATE NhanVien SET TenNhanVien = @Ten, LuongCoBan = @Luong WHERE MaNhanVien = @Ma";
            SqlParameter[] parameters = {
                new SqlParameter("@Ma", maNV),
                new SqlParameter("@Ten", tenNV),
                new SqlParameter("@Luong", luong)
            };

            int rows = db.ExecuteNonQuery(sqlUpdate, parameters);
            if (rows > 0)
            {
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }

            ResetStateAfterAction();
        }

        private void ClearInput()
        {
            txtMaNV.Clear();
            txtTenNV.Clear();
            txtLuongCB.Clear();
            txtMaNV.ReadOnly = false;
            txtMaNV.Focus();
        }

        private void ResetStateAfterAction()
        {
            ClearInput();
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            btnThem.Enabled = true;
        }
    }
}