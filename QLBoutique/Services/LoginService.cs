using QLBoutique.ClothingDbContext;
using QLBoutique.Model;
using System.Linq;

namespace QLBoutique.Services
{
    public class LoginService
    {
        private readonly BoutiqueDBContext _context;

        public LoginService(BoutiqueDBContext context)
        {
            _context = context;
        }
        public static string HashPass(string text)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(text);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return string.Concat(hashBytes.Select(b => b.ToString("x2")));
            }
        }


        //public NhanVien? KiemTraDangNhap(string tenDN, string matKhau)
        //{
        //    string hashedPass = HashPass(matKhau); // ← Bổ sung dòng này

        //    //return _context.
        //    //    .FirstOrDefault(nv => nv.Username == tenDN && nv.Password == matKhau);
        //}

    }
}
