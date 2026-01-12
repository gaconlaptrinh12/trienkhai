# Student Analyzer - Phân Tích Điểm Học Sinh

## 📋 Mô Tả Dự Án
Dự án này cung cấp một chương trình Java để phân tích điểm số của học sinh, bao gồm:
- Đếm số lượng học sinh đạt loại Giỏi (điểm ≥ 8.0)
- Tính điểm trung bình hợp lệ (từ 0 đến 10)

## 🎯 Yêu Cầu Chức Năng

### 1. Lớp `StudentAnalyzer`
Chứa hai phương thức chính:

#### `countExcellentStudents(List<Double> scores)`
- **Mô tả**: Đếm số học sinh đạt loại Giỏi (≥ 8.0)
- **Tham số**: Danh sách điểm từ 0 đến 10
- **Trả về**: Số học sinh đạt loại Giỏi
- **Xử lý**: Bỏ qua điểm âm hoặc > 10; trả về 0 nếu danh sách rỗng

#### `calculateValidAverage(List<Double> scores)`
- **Mô tả**: Tính điểm trung bình của các điểm hợp lệ
- **Tham số**: Danh sách điểm
- **Trả về**: Điểm trung bình
- **Xử lý**: Bỏ qua điểm không hợp lệ; trả về 0 nếu không có điểm hợp lệ

## 🛠️ Công Nghệ Sử Dụng
- **Java 11**
- **JUnit 5** - Framework kiểm thử
- **Maven** - Build tool

## 📦 Cấu Trúc Dự Án
```
StudentAnalyzer/
├── src/
│   ├── main/java/com/example/
│   │   └── StudentAnalyzer.java      # Lớp chính
│   └── test/java/com/example/
│       └── StudentAnalyzerTest.java  # Bài kiểm thử
├── pom.xml                           # Cấu hình Maven
└── README.md                         # Tài liệu này
```

## 🚀 Hướng Dẫn Sử Dụng

### Biên dịch dự án
```bash
mvn clean compile
```

### Chạy kiểm thử
```bash
mvn test
```

### Xem chi tiết kết quả test
```bash
mvn test -X
```

## 📊 Bài Kiểm Thử

Dự án bao gồm **14 test case** bao quát các trường hợp:

### Test cho `countExcellentStudents()`
1. ✅ Trường hợp bình thường - danh sách có dữ liệu hợp lệ và không hợp lệ
2. ✅ Danh sách toàn bộ hợp lệ
3. ✅ Danh sách trống
4. ✅ Danh sách null
5. ✅ Giá trị biên (0, 10)
6. ✅ Dữ liệu không hợp lệ (< 0 hoặc > 10)
7. ✅ Không có học sinh giỏi

### Test cho `calculateValidAverage()`
1. ✅ Trường hợp bình thường - danh sách hỗn hợp
2. ✅ Danh sách toàn bộ hợp lệ
3. ✅ Danh sách trống
4. ✅ Danh sách null
5. ✅ Giá trị biên (0, 10)
6. ✅ Tất cả dữ liệu không hợp lệ
7. ✅ Chỉ một điểm hợp lệ

## 🔄 Quy Trình Git và Issue Tracking

### Kết nối Commit với Issue
Mỗi commit được liên kết với issue tương ứng:

```bash
# Format: <type>: <description> #<issue-number>
git commit -m "feat: implement countExcellentStudents() #1"
git commit -m "feat: implement calculateValidAverage() #2"
git commit -m "test: add unit tests for both methods #3"
git commit -m "docs: update README with instructions #4"
```

### Từ khóa đóng tự động
Sử dụng các từ khóa sau để tự động đóng issue khi merge:
- `fixes #<issue-number>`
- `closes #<issue-number>`
- `resolves #<issue-number>`

**Ví dụ:**
```bash
git commit -m "fixes #1: Hoàn tất triển khai countExcellentStudents()"
```

## 👤 Tác Giả
Sinh viên

## 📝 License
MIT
