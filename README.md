# Robocode Tank Royale - Greedy Bots Collection

Repositori ini berisi sekumpulan bot  untuk game **[Robocode Tank Royale](https://robocode.dev/)** yang dikembangkan menggunakan bahasa **C# (.NET)**. Proyek ini dibuat untuk memenuhi Tugas Besar mata kuliah Strategi Algoritma di Institut Teknologi Sumatera.

Tujuan utama dari proyek ini adalah mengimplementasikan **Algoritma Greedy** untuk memenangkan pertempuran dengan mengoptimalkan komponen skor akhir (Survival, Bullet Damage, dan Kill Bonus).

## Bot & Strategi Heuristik

Terdapat 4 bot dalam repositori ini, dengan **Runtuh** sebagai bot utama. Setiap bot menggunakan *heuristic* Greedy yang berbeda:

* **Runtuh (Bot Utama) - *Distance Heuristic***
    * **Strategi:** Selalu mengunci dan memprioritaskan musuh dengan jarak terdekat.
    * **Aksi Greedy:** Menggunakan *firepower* maksimal saat jarak musuh sangat dekat tanpa mempertimbangkan cadangan energi, serta memadukannya dengan *Predictive Targeting* dan pergerakan *Circle Strafing*.
* **Killa - *Health/Energy Heuristic***
    * **Strategi:** Mengabaikan jarak dan memprioritaskan target dengan sisa energi (HP) paling rendah.
    * **Aksi Greedy:** Bertujuan secepat mungkin mendapatkan poin dari *kill bonus* dengan mengeksekusi musuh yang sedang sekarat.
* **Sniper - *Speed Heuristic***
    * **Strategi:** Memprioritaskan target yang pergerakannya paling lambat atau sedang berdiam diri.
    * **Aksi Greedy:** Menjamin tingkat akurasi tembakan yang mutlak tinggi dan menghindari pemborosan peluru pada target yang lincah.
* **Koboi - *Aiming Time Heuristic***
    * **Strategi:** Mengincar target yang posisinya paling sejajar dengan moncong meriam saat ini.
    * **Aksi Greedy:** Meminimalkan waktu putar senjata demi penembakan instan sebelum target sempat menghindar.

## Fitur Teknis Lanjutan
Selain Algoritma Greedy, bot di repositori ini juga mengimplementasikan:
- **Non-Blocking Execution:** Memisahkan pergerakan bodi, putaran radar, dan putaran meriam agar berjalan simultan menggunakan `SetTurn...` dan `Go()`.
- **Predictive Targeting:** Menghitung lintasan masa depan musuh menggunakan fungsi trigonometri (`Math.Sin` dan `Math.Cos`) berdasarkan kecepatan dan arah gerak musuh.
- **Radar Wobble/Lock:** Menjaga fokus radar pada satu target prioritas tanpa kehilangan *tracking*.

## Cara Menjalankan
1. Pastikan Anda telah menginstal Robocode Tank Royale UI dan **.NET SDK**.
2. *Clone* repositori ini: `git clone https://github.com/colossiansr/reruntuh.git`
3. Masuk ke direktori bot yang diinginkan, misalnya Runtuh: `cd Runtuh`
4. Jalankan bot: `dotnet run`
5. Buka UI Robocode Tank Royale, pastikan server berjalan, dan bot akan otomatis bergabung ke arena pertempuran.

---
*Dibuat oleh Raymond Kolose Nathanael S. - 124140186 - Strategi Algoritma RA*
