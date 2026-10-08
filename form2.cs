using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace buoi6
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            lblTrangThai.Text = "Sẵn sàng. Hãy chạy Migration trước khi dùng.";
            await TaiDanhSach();
        }

        // ===== READ: Tải danh sách Sinh Viên =====
        private async Task TaiDanhSach()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    List<student> danhSach = await context.Students.ToListAsync();
                    dgvSinhVien.DataSource = null;
                    dgvSinhVien.DataSource = danhSach;
                    lblTrangThai.Text = $"Đã tải {danhSach.Count} sinh viên.";
                    lblTrangThai.ForeColor = Color.DarkGreen;
                }
            }
            catch (Exception ex)
            {
                lblTrangThai.Text = "Lỗi: " + ex.Message;
                lblTrangThai.ForeColor = Color.Red;
            }
        }

        private async void btnTaiLai_Click(object sender, EventArgs e)
        {
            await TaiDanhSach();
        }

        // ===== Hiển thị thông tin dòng đang chọn =====
        private void dgvSinhVien_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSinhVien.CurrentRow?.DataBoundItem is student sv)
            {
                txtHoTen.Text = sv.FullName;
                txtDiem.Text = sv.Grade.ToString();
            }
        }

        // ===== CREATE: Thêm Sinh Viên =====
        private async void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên!");
                return;
            }

            if (!double.TryParse(txtDiem.Text, out double diem))
            {
                MessageBox.Show("Điểm không hợp lệ!");
                return;
            }

            try
            {
                using (var context = new AppDbContext())
                {
                    context.Students.Add(new student { FullName = txtHoTen.Text, Grade = diem });
                    await context.SaveChangesAsync();
                }

                lblTrangThai.Text = "Đã thêm sinh viên mới!";
                lblTrangThai.ForeColor = Color.DarkGreen;
                txtHoTen.Text = "";
                txtDiem.Text = "";
                await TaiDanhSach();
            }
            catch (Exception ex)
            {
                lblTrangThai.Text = "Lỗi: " + ex.Message;
                lblTrangThai.ForeColor = Color.Red;
            }
        }

        // ===== UPDATE: Sửa Sinh Viên =====
        private async void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.CurrentRow?.DataBoundItem is not student svDangChon)
            {
                MessageBox.Show("Vui lòng chọn một dòng để sửa!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên!");
                return;
            }

            if (!double.TryParse(txtDiem.Text, out double diemMoi))
            {
                MessageBox.Show("Điểm không hợp lệ!");
                return;
            }

            try
            {
                using (var context = new AppDbContext())
                {
                    student sv = await context.Students.FindAsync(svDangChon.Id);
                    if (sv != null)
                    {
                        sv.FullName = txtHoTen.Text;
                        sv.Grade = diemMoi;
                        await context.SaveChangesAsync();
                    }
                }

                lblTrangThai.Text = "Đã cập nhật thông tin!";
                lblTrangThai.ForeColor = Color.DarkGreen;
                await TaiDanhSach();
            }
            catch (Exception ex)
            {
                lblTrangThai.Text = "Lỗi: " + ex.Message;
                lblTrangThai.ForeColor = Color.Red;
            }
        }

        // ===== DELETE: Xoá Sinh Viên =====
        private async void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.CurrentRow?.DataBoundItem is not student svDangChon)
            {
                MessageBox.Show("Vui lòng chọn một dòng để xóa!");
                return;
            }

            DialogResult ketQua = MessageBox.Show(
                $"Bạn có chắc muốn xóa \"{svDangChon.FullName}\"?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (ketQua != DialogResult.Yes) return;

            try
            {
                using (var context = new AppDbContext())
                {
                    student sv = await context.Students.FindAsync(svDangChon.Id);
                    if (sv != null)
                    {
                        context.Students.Remove(sv);
                        await context.SaveChangesAsync();
                    }
                }

                lblTrangThai.Text = "Đã xóa sinh viên!";
                lblTrangThai.ForeColor = Color.DarkGreen;
                txtHoTen.Text = "";
                txtDiem.Text = "";
                await TaiDanhSach();
            }
            catch (Exception ex)
            {
                lblTrangThai.Text = "Lỗi: " + ex.Message;
                lblTrangThai.ForeColor = Color.Red;
            }
        }

        // ===== LINQ: Lọc Sinh Viên Đạt (Điểm >= 5) =====
        private async void btnLocDat_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    List<student> ketQua = await context.Students
                        .Where(sv => sv.Grade >= 5)
                        .OrderByDescending(sv => sv.Grade)
                        .ToListAsync();

                    dgvSinhVien.DataSource = null;
                    dgvSinhVien.DataSource = ketQua;
                    lblTrangThai.Text = $"Tìm thấy {ketQua.Count} sinh viên đạt (>=5).";
                    lblTrangThai.ForeColor = Color.DarkGreen;
                }
            }
            catch (Exception ex)
            {
                lblTrangThai.Text = "Lỗi: " + ex.Message;
                lblTrangThai.ForeColor = Color.Red;
            }
        }
    }
}
