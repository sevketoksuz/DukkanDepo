# DukkanDepo Handoff

## Proje Özeti

Bu proje eski `MuhasebeWPF` projesinden temizlenerek oluşturulan yeni WPF masaüstü uygulamasıdır.

Yeni proje adı:

```text
DukkanDepo
```

Amaç:

* Eski karmaşık projeyi temiz mimariyle yeniden kurmak
* Eski `MuhasebeWPF` isimlendirmelerini kaldırmak
* Yazlık ve kışlık ürün stok/muhasebe takibini daha düzenli hale getirmek
* SQLite tabanlı local database kullanmak
* Excel/PDF export-import, backup/restore, appearance/theme, özet ve grafik raporlarını korumak

---

## Teknoloji

* .NET 10
* WPF
* SQLite
* Entity Framework Core
* ClosedXML
* QuestPDF
* LiveChartsCore
* ModernWpfUI

Target framework:

```xml
<TargetFramework>net10.0-windows10.0.19041.0</TargetFramework>
```

---

## Mevcut Durum

Bugün planlanan ana parçalar tamamlandı.

Tamamlananlar:

* Domain katmanı oluşturuldu
* `Urun`, `YazlikUrun`, `KislikUrun` entity’leri düzenlendi
* `INotifyPropertyChanged` ile `Tutar` ve `IskontoluTutar` anlık güncellenir hale getirildi
* EF Core SQLite persistence katmanı kuruldu
* Yeni initial migration oluşturuldu
* Database path `DukkanDepo` adına taşındı
* Repository yapısı eklendi
* Backup servisleri eklendi
* PDF export servisleri eklendi
* Excel export/import servisleri eklendi
* Summary ve Charts data provider servisleri eklendi
* Appearance/theme servisleri eklendi
* Launcher window eklendi
* Appearance window eklendi
* Summary window eklendi
* Charts window eklendi
* Yazlık ürün ekranı eklendi
* Kışlık ürün ekranı eklendi
* Geçici navigation kaldırıldı
* Gerçek `LauncherNavigationService` eklendi
* Yazlık/Kışlık grid edit davranışı düzeltildi
* Satış min/max ve iskonto min/max filtreleri kaldırıldı
* Cins ve Model filtre sırası düzeltildi
* Adet/Satış/İskonto kolonlarında klavyeden sayı ile edit açma düzeltildi
* Mouse tek tık edit davranışı kaldırıldı
* Esc exception sorunu düzeltildi
* Grid edit davranışı daha stabil hale getirildi

---

## Manuel Test Durumu

Aşağıdaki testler manuel olarak yapıldı ve şu an tamam kabul ediliyor:

```text
Yazlık ekran manuel testleri: tamam
Kışlık ekran manuel testleri: tamam
Excel import/export: şu anlık tamam
PDF export: tamam
Backup/restore: tamam
Summary window: tamam
Charts window: tamam
```

Not:

```text
Excel import/export tarafı şimdilik temel olarak bırakıldı.
Bu alan şu anda ana geliştirme konusu değil.
```

Şu an düşünülmeyen konu:

```text
YazlikWindow/KislikWindow ortaklaştırması şimdilik düşünülmüyor.
```

---

## Önemli Mimari Notlar

Katmanlar:

```text
Domain
Application
Infrastructure
Presentation
```

Domain:

```text
Domain/Entities/Urun.cs
Domain/Entities/YazlikUrun.cs
Domain/Entities/KislikUrun.cs
```

Persistence:

```text
Infrastructure/Persistence/Context/AppDbContext.cs
Infrastructure/Persistence/Context/AppDbContextFactory.cs
Infrastructure/Persistence/Repositories/
Infrastructure/Persistence/Migrations/
```

Presentation:

```text
Presentation/Views/
Presentation/ViewModels/
Presentation/Services/
Presentation/Helpers/
Presentation/Commands/
Presentation/Converters/
Presentation/Behaviours/
```

---

## Database

Database dosyası şurada oluşur:

```text
%AppData%/DukkanDepo/app.db
```

Genelde Windows path:

```text
C:\Users\<kullanıcı>\AppData\Roaming\DukkanDepo\app.db
```

Migration komutları Visual Studio Package Manager Console üzerinden çalıştırıldı:

```powershell
Add-Migration InitialCreate -OutputDir Infrastructure/Persistence/Migrations
Update-Database
```

`AppDbContext.EnsureMigrate()` uygulama başlangıcında migration çalıştırır.

---

## Grid Edit Davranışı

`Kod`, `Cins`, `Model` kolonları `DataGridTextColumn`.

`Adet`, `Satış`, `İskonto` kolonları `DataGridTemplateColumn`.

Sebep:

* Normal görünümde para değerlerinde `$` göstermek gerekiyor
* Edit modunda `$` TextBox içine girmemeli
* Bu yüzden `CellTemplate` ve `CellEditingTemplate` ayrıldı

Son karar:

```text
Mouse tek tık = sadece hücre seçer
Mouse ikinci tık / çift tık = WPF doğal edit davranışı
Klavye sayı = Adet/Satış/İskonto hücresinde edit açar
F2 = edit açar
Enter = commit
Esc = eski değeri geri yazar ve commit ile çıkar
```

Bu davranışta kullanılan helper:

```text
Presentation/Helpers/DataGridTemplateEditHelper.cs
```

Esc için `CancelEdit()` kullanılmıyor. Çünkü WPF DataGrid bazı template column durumlarında `CancelEdit()` sırasında VisualTree exception fırlatabiliyor.

---

## Appearance / Theme Durumu

Appearance sistemi eklendi ve temel olarak çalışıyor.

Şu an bilinen sorun:

```text
Dark theme seçilince bazı UI elemanları görünmüyor veya okunmuyor.
```

Muhtemel sebepler:

* Bazı `TextBlock`, `Border`, `DataGrid`, `Button`, `Window` veya card stillerinde hard-coded renk kullanılmış olabilir
* Bazı renkler `DynamicResource` yerine sabit hex değerle verilmiş olabilir
* Bazı stiller ModernWpf default theme resource’larını ezmiş olabilir
* DataGrid template column içindeki `TextBlock` veya `TextBox` foreground/background ayarları dark theme’e uyumlu olmayabilir
* `SummaryWindow`, `ChartsWindow`, `AppearanceWindow`, `YazlikWindow`, `KislikWindow` içinde özel card/background/border stilleri dark theme ile uyumsuz olabilir

Araştırma notu:

```text
ModernWpf tarafında light/dark theme desteği var.
Yani dark theme teorik olarak mümkün.
Sorun büyük ihtimalle kütüphanenin desteklememesi değil, bizim XAML resource/style kullanımımız.
```

Bugünkü ana işlerden sonra sıradaki öncelik:

```text
Dark theme görünürlük sorununu çöz.
```

Dark theme için bakılacak dosyalar:

```text
App.xaml
Presentation/Services/Appearance/AppearanceApplier.cs
Presentation/Views/MainWindow.xaml
Presentation/Views/YazlikWindow.xaml
Presentation/Views/KislikWindow.xaml
Presentation/Views/SummaryWindow.xaml
Presentation/Views/ChartsWindow.xaml
Presentation/Views/AppearanceWindow.xaml
Presentation/Converters/KarZararToBrushConverter.cs
```

Kontrol edilecekler:

```text
- StaticResource yerine DynamicResource gerekiyor mu?
- Hard-coded Background/Foreground renkleri var mı?
- Border background renkleri theme-aware mi?
- DataGrid selected/focused cell renkleri dark theme’de okunuyor mu?
- TextBox foreground/background dark theme’de okunuyor mu?
- Summary/Charts card background dark theme’de okunuyor mu?
- KarZararToBrushConverter dark theme’de okunur renk döndürüyor mu?
- ModernWpf ThemeManager ayarı doğru uygulanıyor mu?
```

---

## Son Commit Mesajı

Bugünkü son checkpoint commit mesajı:

```text
Fix product windows grid editing and filters
```

Bu commit şunları kapsar:

* Yazlık/Kışlık grid edit davranışı
* Numeric template column klavye edit desteği
* Esc exception düzeltmesi
* Tutar/İskontolu Tutar anlık hesaplama
* Gereksiz filtrelerin kaldırılması
* Cins/Model filtre sırası düzeltmesi

Bu handoff dosyası için önerilen commit mesajı:

```text
Update handoff with manual test status and dark theme TODO
```

---

## Bilinen / Ertelenen Konular

### 1. Dark Theme Sorunu

Öncelik budur.

Problem:

```text
Dark theme seçilince bazı alanlar görünmüyor veya okunmuyor.
```

İlk araştırma yönü:

```text
ModernWpf dark theme destekliyor.
Bu yüzden önce bizim XAML/stil/resource kullanımımız incelenecek.
```

Başlangıç stratejisi:

```text
1. App.xaml resource merge ve ModernWpf resource kullanımını kontrol et
2. AppearanceApplier içinde theme uygulama sırasını kontrol et
3. Hard-coded renkleri ara
4. Background/Foreground verilen tüm yerleri theme-aware resource’a çevir
5. Özellikle DataGrid, Border, TextBlock, TextBox, Button stillerini test et
6. Dark theme’de Yazlık/Kışlık, Summary, Charts ve Appearance pencerelerini tek tek açıp görünmeyen alanları listele
```

### 2. PowerShell Build/Run Sorunu

Visual Studio üzerinden build/run çalışıyor.

PowerShell tarafında daha önce `bin/obj` dosyaları için access denied / application control / file lock tarzı sorunlar görüldü.

Bu konu şimdilik ertelendi.

Notlar:

* VS Run çalışıyor
* PowerShell clean/build bazen `bin` ve `obj` altındaki dosyaları silemiyor
* Muhtemel sebepler:

  * çalışan `DukkanDepo.exe`
  * Visual Studio/MSBuild file lock
  * Windows Defender / Smart App Control / Controlled Folder Access
  * build server cache

Sonra bakılacaksa önce şunlar denenebilir:

```powershell
Get-Process DukkanDepo -ErrorAction SilentlyContinue | Stop-Process -Force
dotnet build-server shutdown
```

Ama şu an ana geliştirme Visual Studio üzerinden devam ediyor.

### 3. YazlikWindow/KislikWindow Ortaklaştırması

Şimdilik düşünülmüyor.

Sebep:

```text
Önce uygulama davranışı stabil hale getiriliyor.
Kod tekrarını azaltma işi daha sonra ele alınabilir.
```

---

## Sonraki İşler

Öncelikli sonraki adımlar:

1. Dark theme görünürlük sorununu araştır ve düzelt
2. ModernWpf dark theme kullanımını doğrula
3. XAML içinde hard-coded renkleri temizle
4. Theme-aware resource kullanımını artır
5. Appearance ayarlarının uygulama genelinde doğru çalıştığını test et
6. Eski `MuhasebeWPF` ismi kaldı mı kontrol et
7. Gereksiz eski dosya/namespace kaldı mı kontrol et
8. `bin/`, `obj/`, `.vs/` gibi dosyaların git’e girmediğini kontrol et
9. Final temizlik sonrası build/run testi yap

---

## Manuel Test Listesi

Tamamlanan manuel testler:

```text
- Yazlık ekran genel kullanım
- Kışlık ekran genel kullanım
- Yeni satır ekleme
- Kod/Cins/Model edit
- Adet edit
- Satış edit
- İskonto edit
- Tutar anlık güncellenmesi
- İskontolu Tutar anlık güncellenmesi
- Enter ile commit
- Esc exception düzeltmesi
- Hücre seçiliyken sayı basınca edit açılması
- F2 ile edit açılması
- Delete ile satır silme
- Filtrelerin çalışması
- Sayfalama
- Sıralama
- Excel export
- Excel import
- PDF export
- Backup
- Restore
- Özet penceresi
- Grafik penceresi
```

Dark theme için yapılacak manuel testler:

```text
- MainWindow dark theme görünümü
- YazlikWindow dark theme görünümü
- KislikWindow dark theme görünümü
- SummaryWindow dark theme görünümü
- ChartsWindow dark theme görünümü
- AppearanceWindow dark theme görünümü
- DataGrid selected row/cell görünümü
- DataGrid editing TextBox görünümü
- Button/TextBox/ComboBox/DatePicker görünümü
- Card/Border background görünümü
- Kar/Zarar renklerinin okunabilirliği
```

---

## Git Notları

Commit atmadan önce şunların git’e girmediğini kontrol et:

```text
bin/
obj/
.vs/
*.user
*.suo
```

Önerilen `.gitignore` bunları dışarıda bırakmalı.

Handoff güncellemesi için commit:

```powershell
git add handoff.md
git commit -m "Update handoff with manual test status and dark theme TODO"
```

---

## Devam İçin Kısa Özet

Yeni chatte devam ederken şu cümle yeterli olur:

```text
DukkanDepo WPF/.NET 10 projesinde eski MuhasebeWPF projesini temiz repo olarak yeniden kuruyoruz. Domain, EF SQLite, migration, backup/restore, export/import, appearance, launcher, YazlikWindow ve KislikWindow tamamlandı. Manuel testlerde yazlık/kışlık ekranlar, Excel import/export, PDF export, backup/restore, summary ve charts tamam. Şu an ana sorun dark theme seçilince bazı UI elemanlarının görünmemesi. ModernWpf dark theme destekliyor gibi görünüyor; sıradaki iş XAML/resource/style tarafında dark theme uyumluluğunu düzeltmek.
```
