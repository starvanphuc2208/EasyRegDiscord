using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy.MCommon
{
    public class RandomStr
    {
        private static Random random = new Random();

        public static string RandomStringStr(int length)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToLower(), length).Select<string, char>((Func<string, char>)(s => s[random.Next(s.Length)])).ToArray<char>());
        }

        public static string RandomNumberStr(int length)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat<string>("0123456789".ToLower(), length).Select<string, char>((Func<string, char>)(s => s[random.Next(s.Length)])).ToArray<char>());
        }

        public static string FirstName(string country)
        {
            string str = country;
            return !(str == "english") ? (str == "vietnamese" ? RandomStr.FirstNameVietnamese() : RandomStr.FirstNameEnglish()) : RandomStr.FirstNameEnglish();
        }

        public static string LastName(string country)
        {
            string str = country;
            return str == "english" ? RandomStr.LastNameEnglish() : (str == "vietnamese" ? RandomStr.LastNameVietnamese() : RandomStr.LastNameVietnamese());
        }

        private static string FirstNameEnglish()
        {
            List<string> list = ((IEnumerable<string>)"Emerald,Cleora,Malka,France,Joellen,Josephine,Todd,Shelly,Dexter,Milagros,Delphine,Darlene,Rayna,Benito,Jacquelynn,Aliza,Thersa,Sherell,Carylon,Jeanine,Leta,Akilah,Chantel,Concha,Soon,Londa,Gussie,Britta,Edmond,Kristine,Talisha,Mallie,Dorla,Beatriz,Natalie,Akiko,Arletta,Asha,Teresia,Charlsie,Phebe,Nicola,Kala,Kandi,Charlette,Terisa,Nichole,Rachell,Ceola,Sandie,Ty,Jadwiga,Terrell,Jalisa,Odessa,Mercedes,Valene,Cinthia,Kymberly,Lauri,Markita,Maryln,Melodee,Wynona,Jodee,Meda,Bao,Mariah,Dot,Clay,Lavona,Arturo,Sunni,Monique,Candida,Deneen,Lani,Carina,Paulene,Rusty,Carma,Madaline,Joycelyn,Sheba,Rene,Lura,Emerita,Leatha,Karie,Dianna,Beverley,Loralee,Katherina,Elaina,Lavern,Sharda,Lynsey,Joya,Geoffrey,Buster,Shanna,Anissa,Robert,Idella,Sherley,Scarlett,Rina,Hassan,Rodolfo,Rosalyn,Jason,Belia,Danelle,Chastity,Chasidy,Michael,Oliva,Major,Marcie,Madelaine,Barbera,Tereasa,Merlin,Jeanmarie,Quincy,Zada,Candance,Ellyn,Alba,Trinidad,Lelah,Tania,Trudy,Fidel,Wenona,Garret,Sha,Sierra,Maire,Santina,Dannette,Rashida,Erich,Lawanda,Rosamaria,Reena,Fredia,Claribel,Mariam,Ana,Charis,Avelina,Kenya,Eileen,Nikole,Tandra,Ivy,Ronna,Noelle,Gertude,Kory,Lezlie,Enrique,Chanel,Lachelle,Sheldon,Dayna,Denis,Errol,Hilton,Shae,Chandra,Lyle,Lawana,Golden,Brent,Jeff,Magen,Harlan,Hugo,Charlie,Francina,Elenor,Cassidy,Casimira,Amal,Dione,Lois,Kindra,Isaias,Alfreda,Adah,Brinda,Emory,Audra,Lourdes,Flor,Trevor,Caryn,Dusti,Ardelia,Kortney,Veronika,Marlo,Dwana,Roxana,Kimi,Lakita,Susan,Kristy,Nakia,Freda,Evelyne,Katherine,Joslyn,Nelly,Jack,Ossie,Sharita,Lashawn,Sheilah,Andra,Johnette,Marge,Raquel,Ulysses,Tanya,Kori,Aletha,Valery,Norma,Cherry,Charlesetta,Ashly,Aundrea,Efrain,Mozell,Mckinley,Yung,Shanae,Faye,Nicole,Delana,Marissa,Francis,Cliff,Tonya,Dominique,Bart,Fred,Caren,Dylan,Chad,Farrah,Donald,Wendi,Lindsay,Molly,Altagracia,Olimpia,Mikki,Devora,Tricia,Jarvis,Gary,Wilfredo,Leslie,Kaley,Nora,Twanda,Pamela,Shameka,Dori,Romona,Sunshine,Zelma,Lieselotte,Vinita,Nadine,Helga,Sachiko,Long,Barton,Earlene,Teodora,Cristine,Bernardo,Georgiana,Terrance,Tajuana,Mirta,Sebastian,Quintin,Laronda,Riva,Renetta,Coreen,Maple,Fermin,Tiffany,Kandra,Teena,Joana,Lynne,Leigh,Latashia,Cinda,Werner,Janelle,Georgeann,Holli,Adelina,Alta,Corrina,Ima,Illa,Charisse,Cordell,Nolan,Antonetta,Laurene,Andree,Paz,Sharonda,Alyse,Bennett,Maricela,Walker,Nikia,Vina,Lyla,Sammie,Jordon,Patti,Antone,Theodore,Mozella,Ernestina,Irmgard,Isidra,Brunilda,Matt,Milan,Agnus".Split(',')).ToList<string>();
            return list[RandomStr.random.Next(list.Count)];
        }

        private static string LastNameEnglish()
        {
            List<string> list = ((IEnumerable<string>)"Rhynes,Garriga,Bergin,Ziebarth,Mchaney,Steitz,Thiele,Banaszak,Kitterman,Linsley,Lyall,Cannata,Labrum,Hultman,Strange,Marchan,Dirksen,Arendt,Houser,Hackworth,Sanderlin,Brookshire,Lemasters,Halladay,Kuehn,Keena,Koch,Smither,Mahony,Jessen,Kinnaman,Husted,Biehl,Pletcher,Schooler,Roehl,Petro,Adamson,Tavernia,Hamill,Slaven,Figueroa,Waites,Sarris,Doane,Archie,Slovak,Bohannon,Romine,Aucoin,Pendergast,Garmon,Cadle,Benbow,Kinley,Raia,Mckenny,Schueler,Roussel,Mobley,Paschal,Adrian,Persinger,Tilghman,Leng,Allmon,Vass,Seitz,Nelligan,Clutter,Sheehy,Hurd,Horgan,Alward,Mcmackin,Smeltzer,Hoyte,Worcester,Ridley,Sponsler,Daub,Nye,Hurrell,Howey,Whitlatch,Shoemake,Ogata,Dimaio,Pitter,Dishon,Mcgavock,Mastroianni,Batton,Thurber,Yuan,Stetson,Frison,Mincey,Bracy,Witte,Nogle,Balsamo,Dunavant,Halls,Morado,Saba,Stills,Maxey,Friedland,Sheffer,Eder,Oboyle,Chiou,Kelley,Barone,Maness,Booe,Weinstock,Harnois,Pimental,Reiher,Fava,Batchelder,Agarwal,Pekar,Lockridge,Strauss,Biffle,Monterrosa,Aceuedo,Gaspard,Cassell,Shay,Filson,Ridgell,Ludden,Stgermain,Martina,Eberly,Clingan,Gunning,Sink,Craft,Norfleet,Sieren,Henslee,Rick,Lago,Migues,Seaton,March,Bizier,Chee,Talton,Sancho,Mccool,Bustillos,Seats,Jun,Milhorn,Lanclos,Matthews,Meier,Meis,Emmanuel,Holdridge,Senegal,Weigel,Sepeda,Behrens,Crossland,Krejci,Villalta,Mcclung,Helper,Dudney,Beene,Byram,Costanzo,Yaeger,Deck,Cascio,Hon,Fuhrman,Kilduff,Paisley,Seale,Kost,Mincy,Croskey,Howle,Shepherd,Polly,Marten,Bennett,Radabaugh,Castello,Stipe,Joye,Uchida,Stacy,Yates,Reel,Folk,Feola,Caryl,Bias,Birden,Cunningham,Heath,Rancourt,Lucas,Polansky,Mckeehan,Arias,Nicoletti,Paradiso,Roseboro,Veatch,Rezentes,Brammer,Ruder,Schell,Harbert,Merlino,Wentzell,Wahl,Banker,Hardcastle,Osornio,Hufnagel,Garlick,Lanphear,Derksen,Engstrom,Connors,Maurer,Baggs,Mengel,Goehring,Luster,Ringer,Stamant,Tosh,Bostic,Linke,Alejandro,Ensminger,Crane,Mahoney,Cowher,Gramling,Czajkowski,Gendron,Seat,Segundo,Revard,Tone,Garzon,Zaleski,Bruning,Cromwell,Kauppi,Nava,Wolski,Mickelson,Seibert,Yerian,Blaney,Morello,Rivard,Cardillo,Liebold,Thoman,Engelhard,Westra,Benitez,Scherrer,Naquin,Devereaux,Carn,Im,Wilhoite,Neves,Markow,Samsel,Pitzer,Peiffer,Yearta,Pauli,German,Beltrami,Levett,Armor,Racette,Manger,Villicana,Welter,Stallone,Cuccia,Ellingsworth,Horsley,Fender,Erskine,Jefferson,Bouyer,Yarberry,Schall,Mizzell,Shoults,Kovats,Gallimore,Blakeney,Cade,Bobb,Burge,Rollings,Bonneau,Corrado,Gain,Blandon,Lurry,Baize,Stpeter,Winland,Heeren,Bateman,Moya,Vanmeter,Harjo,Ecklund,Uplinger,Cowherd,Boothby,Buchannon,Akers,Sitzman,Frei,Arend,Dutton,Habib,Stukes,Daughtry,Tilley".Split(',')).ToList<string>();
            return list[RandomStr.random.Next(list.Count)];
        }

        private static string FirstNameVietnamese()
        {
            List<string> list = ((IEnumerable<string>)"Liễu,Liệu,Linh,Loan,Long,Lô,Lộc,Lôi,Lời,Lợi,Lụa,Luân,Luận,Luật,Lục,Luỹ,Luyến,Luyện,Lư,Lữ,Lực,Lược,Lương,Lượng,Lưu,Lựu,Ly,Lý,Mai,Mại,Mãn,Mạnh,Mão,Mạo,Mẫn,Mầu,Mậu,Mây,Mịch,Minh,Mít,Mộc,Mưu,Nam,Năm,Năng,Nga,Ngạc,Ngãi,Ngạn,Ngân,Nghê,Nghệ,Nghi,Nghị,Nghĩa,Nghiêm,Nghiên,Nghiệp,Nghinh,Ngọ,Ngoã,Ngoan,Ngoạn,Ngọc,Ngô,Ngộ,Ngôn,Ngũ,Ngụ,Ngung,Nguyên,Nguyện,Nguyệt,Ngữ,Ngự,Nhã,Nhạc,Nham,Nhâm,Nhan,Nhàn,Nhạn,Nhậm,Nhân,Nhẫn,Nhất,Nhật,Nhi,Nhĩ,Nhiệm,Nhiên,Nhiếp,Nhiêu,Nhiễu,Nho,Nhu,Nhuận,Nhuệ,Nhung,Như,Nhự,Nhưỡng,Niêm,Niệm,Niên,Ninh,Nội,Nông,Nữ,Nương,Oai,Oanh,Oánh,Ơn,Phả,Phách,Phái,Phàm,Phạm,Phan,Phán,Phát,Phấn,Phê,Phi,Phiên,Phiến,Phiệt,Phiêu,Phó,Phong,Phóng,Phòng,Phô,Phổ,Phồn,Phu,Phú,Phủ,Phụ,Phúc,Phục,Phùng,Phụng,Phức,Phước,Phương,Phượng,Qua,Quá,Quả,Quan,Quán,Quản,Quang,Quảng,Quân,Quận,Quế,Quốc,Quít,Quy,Quý,Quỳ,Quyên,Quyến,Quyền,Quyết,Quýnh,Quỳnh,Sách,Sám,San,Sang,Sáng,Sanh,Sảnh,Sao,Sát,Sắc,Sâm,Sinh,Sĩ,Siêu,Sinh,Sính,Soạn,Song,Sở,Sơn,Sung,Sử,Sứ,Sự,Sương,Sửu,Tá,Tạ,Tác,Tài,Tác,Tam,Tám,Tạo,Tăng,Tâm,Tầm,Tân,Tấn,Tần,Tập,Tất,Tây,Tế,Thạch,Thaí,Thảng,Thanh,Thành,Thạnh,Thao,Thảo,Thăng,Thắng,Thân,Thận,Thập,Thâu,Thế,Thể,Thi,Thí,Thì,Thị,Thích,Thiên,Thiện,Thiệp,Thiết,Thinh,Thịnh,Thọ,Thoa,Thoả,Thoại,Thoàn,Thôi,Thông,Thống,Thời,Thu,Thủ,Thụ,Thuấn,Thuần,Thuận,Thuật,Thúc,Thục,Thuý,Thuỳ,Thuỷ,Thuỵ,Thuyên,Thuyết,Thư,Thứ,Thừa,Thức,Thực,Thước,Thược,Thương,Thường,Thưởng,Thượng,Thy,Tích,Tiêm,Tiềm,Tiên,Tiến,Tiển,Tiễn,Tiếp,Tiết,Tiêu,Tiếu,Tín,Tinh,Tính,Tình,Tỉnh,Tĩnh,Toa,Toả,Toại,Toan,Toán,Toàn,Toản,Toát,Tòng,Tô,Tố,Tộ,Tổ,Tôn,Tồn,Tống,Tốt,Trà,Trác,Trạc,Trạch,Trai,Trang,Tráng,Tràng,Trạng,Trát,Trắc,Trâm,Trầm,Trân,Trấn,Trần,Tri,Trí,Trì,Trị,Trích,Triêm,Triển,Triết,Triều,Triệu,Trinh,Trình,Trịnh,Trọng,Trợ,Trụ,Truật,Trúc,Trung,Truyền,Trữ,Trứ,Trực,Trưng,Trừng,Trước,Trương,Trường,Trưởng,Tú,Tuân,Tuấn,Tuần,Túc,Tuế,Tuệ,Tung,Tùng,Tụng,Tuy,Tuý,Tuỵ,Tuyên,Tuyến,Tuyền,Tuyển,Tuyết,Tư,Tứ,Từ,Tự,Tước,Tương,Tường,Tưởng,Tửu,Tựu,Ty,Tý,Tỵ,Uẩn,Uy,Uỷ,Uyên,Uyển,ƯÔng,Ưu,Vang,Văn,Vân,Vệ,Vị,Viên,Viễn,Viện,Việt,Vinh,Vĩnh,Vịnh,Võ,Vọng,Vũ,Vui,Vỹ,Vương,Vượng,Xa,Xá,Xoài,Xuân,Xuyên,Xuyến,Xương,Xướng,Yêm,Yểm,Yên,Yến".Split(',')).ToList<string>();
            return list[RandomStr.random.Next(list.Count)];
        }

        private static string LastNameVietnamese()
        {
            List<string> list = ((IEnumerable<string>)"Ái,Ấm,An,Án,Anh,Ánh,Ân,Ấn,Ẩn,Ấp,Ất,Âu,Ấu,Ba,Bá,Bạ,Bạc,Bách,Bạch,Bái,Bài,Ban,Bản,Bàng,Bảng,Báo,Bào,Bảo,Bạt,Báu,Bắc,Bằng,Bặt,Bân,Bật,Bến,Bền,Bi,Bích,Biên,Biền,Biện,Biểu,Bính,Bình,Bố,Bổ,Bộ,Bốc,Bộc,Bôi,Bối,Bồi,Bội,Bôn,Bốn,Bổn,Bông,Bồng,Bổng,Bột,Bùi,Buông,Bút,Bưng,Bưu,Ca,Cai,Cam,Can,Canh,Cảnh,Cán,Cao,Cảo,Cát,Căn,Cầm,Cẩm,Cần,Cẩn,Cận,Cật,Câu,Cầu,Chanh,Chánh,Chăm,Châm,Chân,Chấn,Chẩn,Chấp,Chất,Châu,Chế,Chi,Chí,Chỉ,Chiêm,Chiếm,Chiến,Chiêu,Chiếu,Chinh,Chính,Chình,Chỉnh,Chu,Chú,Chủ,Chúa,Chúc,Chung,Chủng,Chuyên,Chuyển,Chư,Chử,Chức,Chước,Chương,Chưởng,Cổ,Côn,Cổn,Công,Cống,Cơ,Cú,Cù,Cúc,Cung,Củng,Cư,Cứ,Cừ,Cử,Cự,Cương,Cường,Cửu,Cữu,Cựu,Danh,Dao,Dân,Dần,Dẫn,Dật,Di,Dị,Dịch,Diêm,Diễm,Diệm,Diếp,Diệp,Diệu,Dinh,Dĩnh,Do,Doãn,Doanh,Du,Dũ,Dụ,Duật,Dục,Duệ,Dung,Dũng,Dụng,Duy,Duyên,Duyệt,Dư,Dự,Dực,Dược,Dương,Dưỡng,Ðạc,Ðãi,Ðại,Ðam,Ðàm,Ðảm,Ðạm,Ðan,Ðán,Ðàn,Ðản,Ðào,Ðang,Ðàng,Ðảng,Ðào,Ðảo,Ðạo,Ðạt,Ðắc,Ðăng,Ðằng,Ðẳng,Ðẩu,Ðậu,Ðể,Ðệ,Ðiềm,Ðiền,Ðiệp,Ðiều,Ðiểu,Ðinh,Ðính,Ðình,Ðỉnh,Ðĩnh,Ðịnh,Ðoá,Ðoài,Ðoái,Ðoan,Ðoàn,Ðô,Ðổ,Ðộ,Ðốc,Ðối,Ðôn,Ðông,Ðồng,Ðổng,Ðới,Ðơn,Ðủ,Ðức,Ðược,Ðương,Ðường,Em,Gia,Giá,Giả,Giác,Giai,Giải,Giám,Gián,Giản,Giang,Giảng,Giao,Giáo,Giáp,Giàu,Giới,Giũ,Hà,Hạ,Hạc,Hách,Hai,Hài,Hải,Hàm,Hán,Hàn,Hãn,Hạn,Hạng,Hanh,Hành,Hạnh,Hào,Hảo,Hạo,Hạp,Hạt,Hằng,Hân,Hậu,Hiến,Hiền,Hiển,Hiệp,Hiệt,Hiếu,Hiểu,Hiệu,Hinh,Hoa,Hoá,Hoà,Hoạch,Hoài,Hoán,Hoàn,Hoàng,Hoành,Hoạnh,Học,Hoạt,Hồ,Hổ,Hộ,Hối,Hồi,Hội,Hồng,Hợp,Huân,Huấn,Huề,Huệ,Hùng,Huy,Huyên,Huyến,Huyền,Huyện,Huynh,Huỳnh,Hứa,Hưng,Hương,Hướng,Hường,Hưởng,Hưu,Hữu,Hựu,Hy,Hỷ,Ích,Keo,Kế,Kết,Kha,Khá,Khả,Khai,Khái,Khải,Kham,Khán,Khang,Kháng,Khảng,Khanh,Khánh,Khảo,Khắc,Khâm,Khẩn,Khấp,Khâu,Khê,Khiêm,Khiết,Khiêu,Khinh,Khoa,Khoá,Khoả,Khoách,Khoái,Khoan,Khoán,Khoát,Khôi,Khôn,Khổng,Khu,Khúc,Khuê,Khuông,Khuyến,Khuyết,Khuynh,Khương,Khưu,Kiêm,Kiểm,Kiệm,Kiên,Kiện,Kiệt,Kiều,Kim,Kinh,Kính,Kỉnh,Kỳ,Kỷ,Kỵ,La,Lạc,Lai,Lài,Lại,Lam,Lãm,Lan,Lang,Lãng,Lanh,Lành,Lãnh,Lạp,Lăng,Lâm,Lân,Lập,Lâu,Lê,Lễ,Lệ,Lịch,Liêm,Liên,Liễn,Liêu".Split(',')).ToList<string>();
            return list[RandomStr.random.Next(list.Count)];
        }
    }
}
