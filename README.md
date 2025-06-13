## 🧩 About the Project
Một prototype game nhịp điệu đơn giản được xây dựng bằng Unity. Người chơi cần **nhấn đúng thời điểm** khi tile rơi xuống để ghi điểm. Game kết thúc nếu người chơi bỏ lỡ một tile.

---

## ▶️ How to Run the Project

1. **Yêu cầu:**
   - Unity 2021.3.xxx LTS (2021.3.45f1)
   - Hệ điều hành: Windows/macOS

2. **Hướng dẫn chạy:**
   - Clone hoặc giải nén project về máy.
   - Mở Unity Hub → chọn "Open" → chọn thư mục project.
   - Chạy scene `MenuScene`.
   - Nhấn **Play** trong Unity Editor để bắt đầu test.
   - Hoặc build ra Android.

---

## ⚙️ Design Choices

### 1. **Object Pooling**
- Để tối ưu hiệu năng, tile được quản lý bằng hệ thống **Tile Pooling** nhằm tái sử dụng object thay vì Instantiate/Destroy liên tục.
- Có thể dễ dàng mở rộng thêm các loại tile khác (dài/ngắn/đặc biệt).

### 2. **Touch/Click Detection**
- Hệ thống phát hiện vị trí click và xử lý tương ứng với vùng tile rơi.
- Chấm điểm theo độ chính xác: **Perfect**, **Great**, **Cool** và **Miss** nếu rơi xuống vùng khoan an toàn sẽ thành thua.

### 3. **Combo System**
- Combo tăng dần theo chuỗi nhấn chính xác.
- Ngắt combo khi không nhấn đúng thời điểm.

### 4. **Game Flow**
- Game sẽ **Game Over** khi người chơi để tile rơi quá giới hạn.
- Có màn hình hiển thị điểm, combo, hiệu ứng chữ đánh giá điểm (ScoreType).

---

## 🌟 Features Implemented

- ✅ Object Pooling cho tile.
- ✅ Chấm điểm theo độ chính xác so với perfect zone.
- ✅ Combo score tăng dần.
- ✅ Rơi tile theo nhịp
- ✅ Hiệu ứng ngắn (Short Effect) như SFX hoặc particle khi ghi điểm.
- ✅ Hiệu ứng đánh giá (Perfect/Great/Cool).
- ✅ Game Over logic khi tile rơi.
- ✅ Hệ thống quản lý tile và reset khi chơi lại.
- ✅ Hiển thị điểm số tổng.
