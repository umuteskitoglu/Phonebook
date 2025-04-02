# Phonebook Projesi

Bu proje, telefon rehberi yönetimi için bir mikro servis mimarisi kullanmaktadır. Contact Service API ve Report Service API olmak üzere iki ana servis içerir. 

## Sistem Gereksinimleri

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Proje Mimarisi

Proje aşağıdaki bileşenlerden oluşmaktadır:

- **ContactServiceAPI**: Kişilerin ve iletişim bilgilerinin yönetimini sağlar
- **ReportServiceAPI**: Raporların oluşturulması ve yönetimini sağlar
- **PostgreSQL**: Veritabanı
- **RabbitMQ**: Mesaj kuyruk sistemi

## Docker ile Çalıştırma

### Ön Koşullar

- Docker ve Docker Compose sisteminizde kurulu olmalıdır

### Kurulum ve Çalıştırma

1. Projeyi klonlayın:
   ```bash
   git clone <repo-url>
   cd Phonebook
   ```

2. Docker Compose ile tüm servisleri başlatın:
   ```bash
   docker-compose up -d
   ```

3. Tüm servislerin çalıştığını kontrol edin:
   ```bash
   docker-compose ps
   ```

4. Servislere aşağıdaki adreslerden erişebilirsiniz:
   - Contact Service API: http://localhost:8081
   - Report Service API: http://localhost:8082
   - RabbitMQ Yönetim Arayüzü: http://localhost:15672
     - Kullanıcı adı: user
     - Şifre: password
   - PostgreSQL: localhost:5432
     - Veritabanı: PhonebookDb
     - Kullanıcı adı: postgres
     - Şifre: postgres

5. Servisleri durdurmak için:
   ```bash
   docker-compose down
   ```

## Yerel Geliştirme Ortamında Çalıştırma

### Veritabanı ve RabbitMQ'yu Docker ile Çalıştırma

Sadece veritabanı ve RabbitMQ'yu Docker ile çalıştırmak için:

```bash
docker-compose up -d postgres rabbitmq
```

### API Projelerini Yerel Olarak Çalıştırma

1. Contact Service API'yi çalıştırma:
   ```bash
   cd ContactServiceAPI
   dotnet run
   ```

2. Report Service API'yi çalıştırma:
   ```bash
   cd ReportServiceAPI
   dotnet run
   ```

## API Endpointleri

### Contact Service API

- `GET /api/Contacts/GetContacts`: Tüm kişileri listeler
- `POST /api/Contacts/GetContactById`: Belirtilen ID'ye sahip kişiyi getirir
- `POST /api/Contacts/CreateContact`: Yeni bir kişi oluşturur
- `DELETE /api/Contacts/DeleteContact`: Belirtilen kişiyi siler
- `POST /api/Contacts/AddContactInformation`: Kişiye iletişim bilgisi ekler
- `DELETE /api/Contacts/DeleteContactInformation`: Kişiden iletişim bilgisi siler
- `POST /api/Contacts/CreateReport`: Konuma göre kişi sayısı raporu oluşturur


### Report Service API

- `GET /api/Reports/GetAllReports`: Tüm raporları listeler
- `POST /api/Reports/GetReportById`: Belirtilen ID'ye sahip raporu getirir

## Veri Kalıcılığı

Docker Compose yapılandırması, hem PostgreSQL hem de RabbitMQ için named volume'lar içerir:

- postgres-data: PostgreSQL veritabanı dosyalarını saklar
- rabbitmq-data: RabbitMQ verilerini saklar

Bu, konteynerler durdurulup yeniden başlatılsa bile verilerinizin korunacağı anlamına gelir.

Volume'ları tamamen kaldırmak ve temiz bir başlangıç yapmak için:
```bash
docker-compose down -v
```

## Proje Mimarisi ve Tasarım Desenleri

Bu projede, aşağıdaki mimari ve tasarım desenleri kullanılmıştır:

### Clean Architecture (Temiz Mimari)

Proje, Clean Architecture prensiplerini takip ederek katmanlara ayrılmıştır:

- **Domain Layer**: Sistemin temel iş modellerini (Entities) ve temel iş kurallarını içerir. Diğer katmanlara bağımlılığı yoktur.
- **Application Layer**: İş mantığını ve kullanım durumlarını (use cases) içerir. Sadece Domain katmanına bağımlıdır.
- **Infrastructure Layer**: Veritabanı, mesajlaşma servisleri gibi dış sistemlerle iletişim kuran kodları içerir.
- **Persistence Layer**: Veritabanı işlemleri ve Entity Framework yapılandırmaları buradadır.
- **API Layer**: Dış dünya ile iletişimi sağlayan API controller'ları içerir.

### CQRS (Command Query Responsibility Segregation)

Veri okuma (Queries) ve yazma (Commands) işlemleri ayrılmıştır:

- **Commands**: Sistemde veri değişikliği yapan işlemler (CreateContact, AddContactInformation vb.)
- **Queries**: Sistemden veri çeken işlemler (GetContacts, GetReportById vb.)

### Mediator Pattern (MediatR)

İsteklerin işlenmesi ve gerekli işleyicilere yönlendirilmesi için MediatR kütüphanesi kullanılmıştır. Bu sayede:

- Bileşenler arasında gevşek bağlantı (loose coupling) sağlanmıştır
- Controller'lar doğrudan servislerle değil, MediatR aracılığıyla iletişim kurar
- Validation, MediatR pipeline behaviors ile uygulanmıştır

### Mikroservis Mimarisi

- **ContactServiceAPI**: Kişiler ve iletişim bilgileri yönetimi
- **ReportServiceAPI**: Rapor oluşturma ve yönetimi

### Message Queue (RabbitMQ)

Servisler arası asenkron iletişim için RabbitMQ kullanılmıştır:
- Rapor oluşturma isteği ContactServiceAPI'den ReportServiceAPI'ye RabbitMQ üzerinden iletilir
- ReportServiceAPI, raporları asenkron olarak işleyerek sistemin yanıt verme süresini iyileştirir
