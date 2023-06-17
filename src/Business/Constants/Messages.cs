using Core.Extensions;

namespace Business.Constants
{
    public static class Messages
    {
        public static string UserNotFound => "Kullanıcı bulunamadı".ToLocale();
        public static string PasswordError=>"Şifre hatalı".ToLocale();
        public static string SuccessfulLogin=>"Sisteme giriş başarılı".ToLocale();
        public static string UserAlreadyExists => "Bu kullanıcı zaten mevcut".ToLocale();
        public static string UserRegistered => "Kullanıcı başarıyla kaydedildi".ToLocale();
        public static string UserEdited => "Kullanıcı başarıyla düzenlendi".ToLocale();
        public static string AccessTokenCreated=>"Access token başarıyla oluşturuldu".ToLocale();

        public static string AuthorizationDenied => "Yetkiniz yok".ToLocale();


        public static string ProductRegistered => "Ürün başarıyla kaydedildi".ToLocale();
        public static string ProductEdited => "Ürün başarıyla düzenlendi".ToLocale();
        public static string ProductDeleted => "Ürün başarıyla silindi".ToLocale();
    }
}
