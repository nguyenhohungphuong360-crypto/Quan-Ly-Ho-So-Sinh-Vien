# 🎯 BỘ CẨM NANG PHỎNG VẤN CHUYÊN SÂU (.NET & DATABASE CONCURRENCY)
> Dành riêng cho dự án **Quản Lý Hồ Sơ Sinh Viên & Hệ Thống Kiểm Soát Giao Tác Đồng Thời (QLHSSV)**.

---

## ⏱️ PHẦN 1: BÀI NÓI 60 GIÂY (ELEVATOR PITCH)
*Khi nhà tuyển dụng hỏi: "Em hãy giới thiệu ngắn gọn về dự án tiêu biểu nhất của em?"*

> **Gợi ý trả lời:**
> "Dạ, dự án tâm đắc nhất của em là **Hệ thống Quản lý Hồ sơ Sinh viên kết hợp Thực nghiệm Kiểm soát Giao tác Đồng thời (Concurrency Control)** viết trên nền tảng **.NET 10** và **SQL Server**.
>
> Ngoài các nghiệp vụ quản lý sinh viên phân tầng theo mô hình **Clean 3-Tier với Dependency Injection**, điểm nhấn đặc biệt nhất của dự án là việc em chủ động nghiên cứu và giải quyết bài toán **tranh chấp dữ liệu trong môi trường nhiều người dùng đồng thời (High Concurrency)**. Cụ thể, em đã mô phỏng trực quan 4 hiện tượng bất thường kinh điển (Dirty Read, Lost Update, Unrepeatable Read, Phantom Read) kèm phương án cô lập bằng các **Transaction Isolation Levels**. 
>
> Đồng thời, em xây dựng giải pháp **chống Deadlock toàn diện ở cả 2 tầng**: Tầng Database dùng giao thức **Ordering All the Items Protocol** để triệt tiêu circular wait, và tầng Application em viết cơ chế **Deadlock Resilience (SqlRetryHelper)** tự động retry với thuật toán **Exponential Backoff + Jitter** khi SQL ném lỗi 1205. Toàn bộ logic lõi đều được bảo vệ bởi **22 Unit Tests (xUnit + Moq)** với độ phủ tuyệt đối."

---

## 🧠 PHẦN 2: 12 CÂU HỎI KỸ THUẬT KINH ĐIỂN & ĐÁP ÁN ĐIỂM 10

### 1. Tại sao em lại quan tâm đến Transaction & Concurrency thay vì chỉ làm CRUD thông thường?
- **Trả lời:** "Hầu hết các ứng dụng khi chạy thực tế (Production) đều có hàng trăm, hàng nghìn người dùng thao tác đồng thời. CRUD cơ bản chỉ hoạt động tốt ở môi trường đơn luồng (Single-thread). Nếu không hiểu về Concurrency, hệ thống sẽ gặp các lỗi cực kỳ nghiêm trọng như: trừ tiền 2 lần, sinh viên đăng ký vượt quá sĩ số lớp, hoặc dữ liệu bị đè mất (Lost Update). Em xây dựng phần này để chứng minh em có tư duy kiến trúc và sẵn sàng làm việc với các hệ thống phân tán chịu tải cao."

---

### 2. Em hãy giải thích 4 hiện tượng bất thường khi giao tác đồng thời (Concurrency Anomalies)?
- **Trả lời:**
  1. **Dirty Read (Đọc rác):** Giao tác $T_1$ sửa dữ liệu nhưng chưa COMMIT. Giao tác $T_2$ đọc dữ liệu đó. Sau đó $T_1$ ROLLBACK. Dữ liệu mà $T_2$ đọc được là dữ liệu rác, không bao giờ tồn tại.
  2. **Non-Repeatable Read (Đọc không lặp lại):** $T_1$ đọc một dòng dữ liệu. $T_2$ sửa hoặc xóa dòng đó rồi COMMIT. $T_1$ đọc lại cùng dòng đó và thấy giá trị đã bị thay đổi.
  3. **Phantom Read (Bóng ma):** $T_1$ truy vấn một tập dữ liệu theo điều kiện (ví dụ: đếm số sinh viên lớp CNTT01 được 30 bạn). $T_2$ INSERT thêm 1 sinh viên mới vào lớp đó rồi COMMIT. $T_1$ thực hiện lại câu lệnh và thấy xuất hiện 31 bạn (dữ liệu "bóng ma" mới xuất hiện).
  4. **Lost Update (Mất cập nhật):** Hai giao tác cùng đọc một bản ghi cùng lúc, sau đó cả hai cùng sửa và ghi đè lên nhau, khiến thay đổi của một bên bị biến mất hoàn toàn.

---

### 3. Các Transaction Isolation Levels trong SQL Server ngăn chặn các hiện tượng trên như thế nào?
- **Trả lời:**
  - `READ UNCOMMITTED`: Cho phép đọc cả dữ liệu chưa commit (chấp nhận Dirty Read). Thường chỉ dùng khi xuất báo cáo thống kê không cần chính xác 100% để tránh giữ khóa.
  - `READ COMMITTED` (Mặc định của SQL Server): Chỉ đọc dữ liệu đã commit, giải quyết được Dirty Read nhưng vẫn bị Non-repeatable Read và Phantom Read.
  - `REPEATABLE READ`: Giữ Shared Lock trên các dòng đã đọc cho tới hết giao tác. Ngăn chặn được Dirty Read và Non-repeatable Read.
  - `SERIALIZABLE`: Mức cô lập cao nhất dùng khóa khoảng (Key-range Lock). Ngăn chặn được tất cả (kể cả Phantom Read), nhưng dễ gây giảm hiệu năng và tăng nguy cơ Deadlock.
  - `SNAPSHOT ISOLATION`: Sử dụng cơ chế kiểm soát đồng thời đa phiên bản (**MVCC - Multi-Version Concurrency Control** lưu trong `tempdb`). Người đọc không khóa người ghi, người ghi không khóa người đọc. Rất tối ưu hiệu năng.

---

### 4. Deadlock là gì? 4 điều kiện Coffman để Deadlock xảy ra?
- **Trả lời:**
  - "Deadlock là trạng thái bế tắc khi hai hay nhiều tiến trình cùng chờ tài nguyên mà tiến trình kia đang nắm giữ, khiến không tiến trình nào có thể tiếp tục.
  - Để Deadlock xảy ra, bắt buộc phải thỏa mãn đồng thời **4 điều kiện Coffman**:
    1. **Mutual Exclusion (Độc quyền):** Tài nguyên không thể chia sẻ đồng thời (ví dụ: Exclusive Lock `X-Lock`).
    2. **Hold and Wait (Giữ và Chờ):** Tiến trình đang giữ tài nguyên và tiếp tục yêu cầu thêm tài nguyên mới.
    3. **No Preemption (Không thể chiếm đoạt):** Không thể ép buộc thu hồi tài nguyên từ một tiến trình đang giữ.
    4. **Circular Wait (Chờ đợi vòng tròn):** Tồn tại một chu trình khép kín: $P_1$ chờ $P_2$, $P_2$ chờ $P_3$,..., $P_n$ chờ $P_1$."

---

### 5. Giao thức "Ordering All Items Protocol" trong dự án của em giải quyết Deadlock như thế nào?
- **Trả lời:**
  - "Giao thức này nhắm thẳng vào việc **phá vỡ điều kiện thứ 4 (Circular Wait)**.
  - Khi hai giao tác $T_1$ và $T_2$ cùng cần thao tác trên hai bảng `Lop` và `SinhVien`:
    - Ở kịch bản gây Deadlock: $T_1$ khóa `Lop` trước rồi chờ khóa `SinhVien`. Cùng lúc đó $T_2$ khóa `SinhVien` trước rồi chờ khóa `Lop` $\rightarrow$ Hình thành Circular Wait $\rightarrow$ SQL Server Deadlock Monitor phải chọn 1 tiến trình làm Victim để Kill.
    - Với **Ordering Protocol**: Toàn bộ hệ thống quy ước một **thứ tự khóa duy nhất (Global Order)**: Bắt buộc luôn luôn khóa bảng `Lop` trước, sau đó mới được khóa bảng `SinhVien`. Nhờ quy ước tuần tự này, $T_2$ sẽ phải đứng chờ ngay từ bảng `Lop`, loại bỏ hoàn toàn khả năng xảy ra chu trình chờ."

---

### 6. Nếu ở tầng Database đã có protocol, tại sao tầng Application vẫn cần `SqlRetryHelper`?
- **Trả lời:**
  - "Trong thực tế microservices hoặc cơ sở dữ liệu lớn, việc kiểm soát thứ tự khóa 100% trên hàng trăm Stored Procedure và ORM là điều không tưởng. Deadlock thỉnh thoảng vẫn có thể xảy ra ngoài ý muốn.
  - Khi SQL Server phát hiện Deadlock, nó sẽ bắn ra mã lỗi **SQL Exception 1205** (*deadlock victim*). Nếu ứng dụng không bắt lỗi này, người dùng sẽ thấy thông báo crash hoặc hệ thống dừng hoạt động.
  - Em đã thiết kế class `SqlRetryHelper` theo mô hình **Transient Fault Handling / Resilience**:
    - Khi bắt được mã lỗi `1205` hoặc `1222`, hệ thống không ném lỗi ngay mà tự động thực thi lại giao tác.
    - Thời gian chờ áp dụng thuật toán **Exponential Backoff ($Delay = Base \times 2^{Attempt}$)** kết hợp **Random Jitter** để các tiến trình không bị xung đột lại ở cùng một phần nghìn giây."

---

### 7. Tại sao em dùng BCrypt thay vì MD5 hay SHA-256 để lưu mật khẩu?
- **Trả lời:**
  - "MD5 và SHA-256 là các hàm băm thông điệp (Message Digest), được thiết kế với tiêu chí **càng tính toán nhanh càng tốt**. Điều này là thảm họa cho mật khẩu vì hacker có thể dùng GPU/ASIC để vét cạn hàng tỷ hash mỗi giây hoặc dùng Rainbow Table.
  - **BCrypt** là thuật toán băm chuyên dụng cho mật khẩu vì:
    1. Tự động sinh Salt ngẫu nhiên và nhúng kèm vào chuỗi hash, vô hiệu hóa hoàn toàn Rainbow Table.
    2. Có tham số **Work Factor (Cost)**: Cho phép điều chỉnh độ trễ tính toán. Dự án của em dùng `WorkFactor = 11`, mỗi lần hash mất khoảng 100-200ms trên CPU. Khoảng thời gian này người dùng không cảm nhận được, nhưng với hacker muốn brute-force thì sẽ mất hàng chục năm."

---

### 8. Cơ chế "Zero-Downtime Migration" cho mật khẩu trong dự án của em hoạt động thế nào?
- **Trả lời:**
  - "Khi tiếp nhận một hệ thống cũ (legacy system) đang lưu mật khẩu thô dạng plaintext hoặc chuẩn băm cũ, ta không thể ép toàn bộ hàng nghìn người dùng reset mật khẩu cùng một lúc.
  - Trong class `PasswordHasher`, em cài đặt phương thức `VerifyAndCheckMigration`:
    - Khi người dùng đăng nhập: Hệ thống kiểm tra xem hash lưu trong DB có tiền tố BCrypt (`$2a$`, `$2b$`) hay không.
    - Nếu là chuỗi plaintext cũ: Hệ thống so sánh chuỗi. Nếu đúng, cho phép đăng nhập thành công, đồng thời trả cờ `needsMigration = true`.
    - Tầng `TaiKhoanBUS` nhận cờ này sẽ lập tức băm mật khẩu thành BCrypt chuẩn và âm thầm cập nhật vào SQL Server.
    - Qua thời gian, toàn bộ tài khoản sẽ tự động chuyển đổi sang chuẩn bảo mật mới mà không gây gián đoạn bất kỳ giây nào."

---

### 9. Tại sao em dùng Dependency Injection (DI) trong một dự án Windows Forms?
- **Trả lời:**
  - "Nhiều người nghĩ WinForms chỉ cần `new` đối tượng. Nhưng việc `new` trực tiếp khiến các tầng phụ thuộc cứng (Tightly Coupled), vi phạm nguyên lý **Dependency Inversion (D trong SOLID)**: Tầng cấp cao (`BUS`) phụ thuộc vào chi tiết tầng cấp thấp (`DAL`).
  - Khi áp dụng DI:
    1. **Tách rời sự phụ thuộc:** `SinhVienBUS` chỉ giao tiếp qua `ISinhVienDAL`.
    2. **Kiểm thử dễ dàng (Testability):** Nhờ có DI, em có thể dùng thư viện `Moq` để giả lập `ISinhVienDAL` trong các bài Unit Test mà không cần phải kết nối database thật.
    3. **Quản lý vòng đời đối tượng (Lifecycle Management):** Dễ dàng cấu hình `Singleton` hay `Transient` qua container."

---

### 10. Em giải thích về Result Pattern và tại sao nên dùng thay vì Exception?
- **Trả lời:**
  - "Trong .NET, việc throw và catch một Exception là một tác vụ rất tốn kém hiệu năng vì CLR phải gom Stack Trace và đi qua các cơ chế xử lý ngoại lệ.
  - Hơn nữa, việc một sinh viên nhập sai định dạng email hay trùng mã sinh viên là **nghiệp vụ dự kiến trước (Expected Business Flow)**, không phải là một lỗi hệ thống bất thường (Exceptional Failure).
  - Em dùng `Result` và `Result<T>` để gói gọn trạng thái thành công/thất bại (`IsSuccess`, `ErrorMessage`, `Data`). Code trở nên tường minh, an toàn với kiểu dữ liệu (strongly-typed) và dễ đọc hơn rất nhiều."

---

### 11. Em tổ chức Unit Test thế nào và có những kịch bản nào nổi bật?
- **Trả lời:**
  - "Dự án của em có project kiểm thử `QLHSSV.Tests` sử dụng **xUnit, Moq và FluentAssertions** với **22 tests pass 100%**.
  - Các bài test tiêu biểu:
    - **Kiểm thử quy tắc nghiệp vụ:** Kiểm tra sinh viên dưới 15 tuổi hoặc trên 80 tuổi, kiểm tra regex định dạng Email và CCCD 9/12 số.
    - **Kiểm thử Mocking:** Dùng `Moq` giả lập trường hợp mã sinh viên đã tồn tại để xác minh rằng phương thức `Insert` của DAL tuyệt đối không bao giờ bị gọi tới.
    - **Kiểm thử cơ chế băm & di trú mật khẩu:** Đảm bảo chuỗi băm chuẩn 60 ký tự và cơ chế `needsMigration` kích hoạt chính xác khi gặp tài khoản cũ."

---

### 12. Nếu được tiếp tục phát triển dự án này lên cấp độ cao hơn, em sẽ bổ sung những gì?
- **Trả lời:**
  - "Dạ, nếu có thêm thời gian, em sẽ mở rộng theo 3 hướng:
    1. **Entity Framework Core & Dapper:** Kết hợp Dapper cho các câu truy vấn hiệu năng cao và EF Core cho việc quản lý migrations.
    2. **RESTful Web API & Docker:** Đóng gói backend thành ASP.NET Core Web API, containerize cơ sở dữ liệu và app bằng Docker Compose để có thể deploy lên Kubernetes/Cloud.
    3. **Redis Caching:** Thêm tầng In-Memory Cache (Redis) cho các danh mục ít thay đổi như Khoa, Ngành để giảm tải truy vấn cho SQL Server."

---

## 🏆 KINH NGHIỆM VÀNG KHI ĐI PHỎNG VẤN
1. **Chủ động mở mã nguồn hoặc demo:** Hãy hỏi người phỏng vấn: *"Dạ em có mang theo source code và demo kịch bản mô phỏng Deadlock / Transaction trực quan, anh/chị có muốn xem qua không ạ?"*. Điều này thể hiện sự tự tin và đam mê 100%.
2. **Nói về quá trình giải quyết vấn đề (Troubleshooting):** Kể về việc ban đầu gặp lỗi Deadlock 1205 ra sao, đã dùng SQL Profiler / Extended Events xem đồ thị Deadlock Graph thế nào, và đã viết `SqlRetryHelper` với Exponential Backoff để khắc phục ra sao.
3. **Từ khóa ghi điểm:** *ACID, Concurrency Anomalies, Isolation Levels, MVCC, Deadlock Prevention, Coffman Conditions, BCrypt Work Factor, Zero-Downtime Migration, SOLID, Dependency Injection, Result Pattern, Resilience Retry.*

