# DukkanDepo Handoff

## Proje Özeti

Bu proje eski `MuhasebeWPF` projesinden temizlenerek oluşturulan yeni WPF masaüstü uygulamasıdır.

Yeni proje adı:

```text
DukkanDepo
```

Amaç:

```text
- Eski karmaşık projeyi temiz repo olarak yeniden kurmak
- Eski MuhasebeWPF isimlendirmelerini kaldırmak
- Yazlık ve kışlık ürün stok/muhasebe takibini daha düzenli hale getirmek
- SQLite tabanlı local database kullanmak
- Excel/PDF import-export, backup/restore, appearance/theme, summary ve chart özelliklerini korumak
```

---

## Teknoloji

```text
- .NET 10
- WPF
- SQLite
- Entity Framework Core
- ClosedXML
- QuestPDF
- LiveChartsCore
- ModernWpfUI
```

Target framework:

```xml
<TargetFramework>net10.0-windows10.0.19041.0</TargetFramework>
```

---

## Mevcut Durum

Bugüne kadar ana uygulama ayağa kaldırıldı ve temel özellikler çalışır hale getirildi.

Tamamlananlar:

```text
- Domain katmanı oluşturuldu
- Urun, YazlikUrun, KislikUrun entity’leri düzenlendi
- INotifyPropertyChanged ile Tutar ve IskontoluTutar anlık güncellenir hale getirildi
- EF Core SQLite persistence katmanı kuruldu
- Initial migration oluşturuldu
- Database path DukkanDepo adına taşındı
- Repository yapısı eklendi
- Backup servisleri eklendi
- PDF export servisleri eklendi
- Excel export/import servisleri eklendi
- Summary ve Charts data provider servisleri eklendi
- Appearance/theme servisleri eklendi
- Launcher/MainWindow eklendi
- AppearanceWindow eklendi
- SummaryWindow eklendi
- ChartsWindow eklendi
- Yazlık ürün ekranı eklendi
- Kışlık ürün ekranı eklendi
- Gerçek LauncherNavigationService eklendi
- Yazlık/Kışlık grid edit davranışı düzeltildi
- Satış min/max ve iskonto min/max filtreleri kaldırıldı
- Cins ve Model filtre sırası düzeltildi
- Adet/Satış/İskonto kolonlarında klavyeden sayı ile edit açma düzeltildi
- Mouse tek tık edit davranışı kaldırıldı
- Esc exception sorunu düzeltildi
- Grid edit davranışı daha stabil hale getirildi
- Dark mode + boş ürün sayfası açılışındaki ArgumentNullException düzeltildi
```

---

## Manuel Test Durumu

Şu ana kadar manuel test edilen ve tamam kabul edilenler:

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
- Dark mode açıkken boş Yazlık/Kışlık ürün sayfası açılışı
- Boş ürün sayfasında 1 adet draft satır gelmesi
- Yeni ürün kaydedilince alta yeni draft satır eklenmesi
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

```text
- Normal görünümde para değerlerinde $ göstermek gerekiyor
- Edit modunda $ TextBox içine girmemeli
- Bu yüzden CellTemplate ve CellEditingTemplate ayrıldı
```

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

## Dark Mode Empty Product Grid Fix

Bugün çözülen kritik hata:

```text
Dark mode açıkken Yazlık/Kışlık ürün sayfası boş veritabanıyla açıldığında
System.ArgumentNullException: Value cannot be null
hatası fırlıyordu.
```

Light mode’da aynı senaryoda hata yoktu.

Tespit edilen sebep:

```text
Dark theme + boş ürün listesi + DataGrid CanUserAddRows=True
```

Bu kombinasyonda WPF’in kendi `NewItemPlaceholder` satırı devreye giriyor. Dark theme render/binding tarafında bu placeholder null/özel obje gibi davranıp exception fırlatabiliyor.

Uygulanan çözüm:

```text
DataGrid CanUserAddRows=False yapıldı.
WPF’in kendi placeholder/new item satırı kullanılmıyor.
Bunun yerine ViewModel içinde manuel draft ürün satırı sistemi kuruldu.
```

Yeni davranış:

```text
Ürün yoksa:
- Ekranda 1 adet boş draft satır gelir.
- Kullanıcı bu satıra ürün bilgisi girer.
- Kod alanı doluysa ürün kaydedilir.
- Kaydedilince alta yeni boş draft satır eklenir.

Ürün varsa:
- Ürünler listelenir.
- En altta 1 adet boş draft satır bulunur.
- Yeni ürün girilince kaydedilir.
- Alta tekrar yeni boş draft satır gelir.
```

Bu sayede eski kullanım hissi korunurken WPF’in problemli placeholder satırı devre dışı bırakıldı.

---

## Dark Mode Empty Product Grid Fix İçin Değişen Dosyalar

```text
Presentation/Views/YazlikWindow.xaml
Presentation/Views/KislikWindow.xaml
Presentation/ViewModels/Main/MainViewModel.cs
Presentation/ViewModels/Main/YazlikWindowViewModel.cs
Presentation/ViewModels/Main/KislikWindowViewModel.cs
Presentation/Views/YazlikWindow.xaml.cs
Presentation/Views/KislikWindow.xaml.cs
Presentation/Converters/SelectedItemSafeConverter.cs
Presentation/Helpers/DataGridEditHelper.cs
```

Özet değişiklikler:

```text
- YazlikWindow/KislikWindow DataGrid CanUserAddRows=False yapıldı
- MainViewModel içine EnsureDraftRow() mantığı eklendi
- MainViewModel içine CanPersistProduct() mantığı eklendi
- VerileriYukle() sonunda draft satır garanti edildi
- SaveRow() içinde Kod boşsa persist engellendi
- Yeni ürün kaydedilince otomatik draft satır eklendi
- Excel export tarafında Id > 0 filtrelemesi yapıldı
- SelectedItemSafeConverter null/unset/placeholder durumlarına karşı güçlendirildi
- DataGridEditHelper.IsPlaceholder() null, NamedObject ve NewItemPlaceholder için güvenli hale getirildi
```

---

## Appearance / Theme Durumu

Appearance sistemi eklendi ve temel olarak çalışıyor.

ModernWpf tarafında light/dark theme desteği var. Bu yüzden dark theme teorik olarak mümkün.

Şu an ana problem:

```text
Dark theme seçilince bazı UI elemanları görünmüyor veya okunmuyor.
```

Muhtemel sebepler:

```text
- Bazı TextBlock, Border, DataGrid, Button, Window veya card stillerinde hard-coded renk kullanılmış olabilir
- Bazı renkler DynamicResource yerine sabit hex değerle verilmiş olabilir
- Bazı stiller ModernWpf default theme resource’larını ezmiş olabilir
- DataGrid template column içindeki TextBlock veya TextBox foreground/background ayarları dark theme’e uyumlu olmayabilir
- SummaryWindow, ChartsWindow, AppearanceWindow, YazlikWindow, KislikWindow içinde özel card/background/border stilleri dark theme ile uyumsuz olabilir
- LiveCharts eksen/label yazıları normal TextBlock olmadığı için ayrıca ele alınabilir
```

Önemli karar:

```text
Global App.xaml içine TextBlock/Label/ToolBar gibi genel stiller ekleme yaklaşımı denenmiş fakat iyi sonuç vermemiştir.
Bu yüzden appearance sorunları global style ile değil, sayfa sayfa çözülecek.
```

---

## Appearance İçin Bakılacak Dosyalar

```text
App.xaml
Presentation/Services/Appearance/AppearanceApplier.cs
Presentation/Views/MainWindow.xaml
Presentation/Views/AppearanceWindow.xaml
Presentation/Views/YazlikWindow.xaml
Presentation/Views/KislikWindow.xaml
Presentation/Views/SummaryWindow.xaml
Presentation/Views/ChartsWindow.xaml
Presentation/Converters/KarZararToBrushConverter.cs
```

Kontrol edilecekler:

```text
- StaticResource yerine DynamicResource gerekiyor mu?
- Hard-coded Background/Foreground renkleri var mı?
- Window background theme-aware mı?
- Border background ve border renkleri theme-aware mı?
- TextBlock foreground dark theme’de okunuyor mu?
- DataGrid selected/focused cell renkleri dark theme’de okunuyor mu?
- DataGrid editing TextBox foreground/background dark theme’de okunuyor mu?
- Button/TextBox/ComboBox/DatePicker görünümü düzgün mü?
- Summary card background ve yazıları dark theme’de okunuyor mu?
- ChartsWindow chart başlıkları ve eksen yazıları dark theme’de okunuyor mu?
- KarZararToBrushConverter dark theme’de okunur renk döndürüyor mu?
- ModernWpf ThemeManager ayarı doğru uygulanıyor mu?
```

---

## Appearance Sorunları İçin Sıradaki Çalışma Planı

Appearance sorunları sayfa sayfa çözülecek.

Önerilen sıra:

```text
1. MainWindow dark theme görünümü
2. AppearanceWindow dark theme görünümü
3. YazlikWindow dark theme görünümü
4. KislikWindow dark theme görünümü
5. SummaryWindow dark theme görünümü
6. ChartsWindow dark theme görünümü
```

Sayfa sayfa ilerlerken yöntem:

```text
- Önce sadece ilgili sayfanın XAML’i değiştirilecek
- Global App.xaml style değişikliklerinden kaçınılacak
- Window background theme-aware yapılacak
- Başlık/label TextBlock renkleri düzeltilecek
- Card/border background ve border renkleri düzeltilecek
- DataGrid selected/editing durumları test edilecek
- En son chart gibi özel kütüphane kontrolleri düzeltilecek
```

---

## Dark Theme Manuel Test Listesi

Appearance çalışmasına geçince şu liste tek tek kontrol edilecek:

```text
- MainWindow dark theme görünümü
- AppearanceWindow dark theme görünümü
- YazlikWindow dark theme görünümü
- KislikWindow dark theme görünümü
- SummaryWindow dark theme görünümü
- ChartsWindow dark theme görünümü
- DataGrid selected row/cell görünümü
- DataGrid editing TextBox görünümü
- Button/TextBox/ComboBox/DatePicker görünümü
- Card/Border background görünümü
- Kar/Zarar renklerinin okunabilirliği
- Chart eksen ve label yazılarının okunabilirliği
```

---

## Bilinen / Ertelenen Konular

### 1. Appearance / Dark Theme Sorunları

Öncelik budur.

Problem:

```text
Dark theme seçilince bazı alanlar görünmüyor veya okunmuyor.
```

Başlangıç stratejisi:

```text
1. MainWindow ile başla
2. Global App.xaml style ekleme
3. Sadece ilgili sayfanın XAML’ini değiştir
4. Hard-coded renkleri temizle
5. DynamicResource kullan
6. Her sayfadan sonra manuel test yap
```

### 2. PowerShell Build/Run Sorunu

Visual Studio üzerinden build/run çalışıyor.

PowerShell tarafında daha önce `bin/obj` dosyaları için access denied / application control / file lock tarzı sorunlar görüldü.

Ayrıca Windows Application Control / Smart App Control tarafında DLL engelleme görüldü:

```text
Could not load file or assembly DukkanDepo.dll
Uygulama Denetimi ilkesi bu dosyayı engelledi.
0x800711C7
```

Bu konu şimdilik ana geliştirme konusu değil.

Notlar:

```text
- VS Run çalışıyor
- PowerShell clean/build bazen bin ve obj altındaki dosyaları silemiyor
- Windows Security / Application Control bazen debug DLL’i engelleyebiliyor
```

Gerekirse denenebilecek komutlar:

```powershell
Get-Process DukkanDepo -ErrorAction SilentlyContinue | Stop-Process -Force
dotnet build-server shutdown
Get-ChildItem "E:\DukkanDepo" -Recurse -Force | Unblock-File
```

### 3. YazlikWindow/KislikWindow Ortaklaştırması

Şimdilik düşünülmüyor.

Sebep:

```text
Önce uygulama davranışı stabil hale getiriliyor.
Kod tekrarını azaltma işi daha sonra ele alınabilir.
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

Önerilen commit mesajı:

```text
Fix dark mode empty product grid crash
```

Commit komutu:

```powershell
git status

git add App.xaml `
        Presentation/ViewModels/Main/MainViewModel.cs `
        Presentation/ViewModels/Main/YazlikWindowViewModel.cs `
        Presentation/ViewModels/Main/KislikWindowViewModel.cs `
        Presentation/Views/YazlikWindow.xaml `
        Presentation/Views/KislikWindow.xaml `
        Presentation/Views/YazlikWindow.xaml.cs `
        Presentation/Views/KislikWindow.xaml.cs `
        Presentation/Converters/SelectedItemSafeConverter.cs `
        Presentation/Helpers/DataGridEditHelper.cs `
        handoff.md

git commit -m "Fix dark mode empty product grid crash"
```

Eğer `App.xaml` son halinde değişmediyse commit’e alınmayabilir.

---

## Sonraki Chat İçin Kısa Başlangıç Cümlesi

```text
DukkanDepo WPF/.NET 10 projesinde dark theme sorunlarını sayfa sayfa çözüyoruz. Dark mode + boş ürün sayfası açılışında gelen ArgumentNullException çözüldü. CanUserAddRows kapatıldı, onun yerine MainViewModel içinde manuel draft ürün satırı sistemi kuruldu. Ürün yoksa 1 boş satır geliyor, yeni ürün kaydedilince otomatik yeni draft satır ekleniyor. Global App.xaml style yaklaşımı kullanılmayacak; appearance problemleri MainWindow’dan başlayarak sayfa sayfa düzeltilecek.
```
