using System;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreType.DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CoreData;

#nullable disable

namespace CoreData.DBContexts
{
    public partial class genesisContext_PostgreSQL : GenesisContextBase
    {
        public genesisContext_PostgreSQL()
        {
        }

        public genesisContext_PostgreSQL(DbContextOptions<genesisContext_PostgreSQL> options)
            : base(options)
        {
        }

        public virtual DbSet<AuthAction> AuthActions { get; set; }
        public virtual DbSet<AuthResource> AuthResources { get; set; }
        public virtual DbSet<AuthTemplate> AuthTemplates { get; set; }
        public virtual DbSet<AuthTemplateDetail> AuthTemplateDetails { get; set; }
        public virtual DbSet<AuthUserRight> AuthUserRights { get; set; }
        public virtual DbSet<CommunicationDefinition> CommunicationDefinitions { get; set; }
        public virtual DbSet<CommunicationTemplate> CommunicationTemplates { get; set; }
        public virtual DbSet<CoreCompany> CoreCompanies { get; set; }
        public virtual DbSet<CoreDepartment> CoreDepartments { get; set; }
        public virtual DbSet<CoreParameter> CoreParameters { get; set; }
        public virtual DbSet<CoreUser> CoreUsers { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationSetting> NotificationSettings { get; set; }
        public virtual DbSet<SampleEmployee> SampleEmployees { get; set; }
        public virtual DbSet<SampleEmployeeTask> SampleEmployeeTasks { get; set; }
        public virtual DbSet<Tenant> Tenants { get; set; }
        public virtual DbSet<TransactionLog> TransactionLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(ConfigurationManager.GetConnectionString("GenesisDB"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasAnnotation("Relational:Collation", "en_US.utf8");

            modelBuilder.Entity<AuthAction>(entity =>
            {
                entity.HasKey(e => e.ActionId);

                entity.ToTable("authActions");

                entity.HasIndex(e => e.ResourceId, "IX_AuthActions_resourceId");

                entity.HasIndex(e => new { e.ResourceId, e.ActionType }, "authActions_resourceId_and_actionType_must_be_unique")
                    .IsUnique();

                entity.Property(e => e.ActionId).HasColumnName("actionId");

                entity.Property(e => e.ActionType).HasColumnName("actionType");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.CreatedUserId).HasColumnName("createdUserId");

                entity.Property(e => e.OrderIndex).HasColumnName("orderIndex");

                entity.Property(e => e.ResourceId).HasColumnName("resourceId");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.TenantId).HasColumnName("tenantId");

                entity.Property(e => e.UpdatedDate).HasColumnName("updatedDate");

                entity.Property(e => e.UpdatedUserId).HasColumnName("updatedUserId");

                entity.HasOne(d => d.Resource)
                    .WithMany(p => p.AuthActions)
                    .HasForeignKey(d => d.ResourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("authActions_resourceId_fkey");
            });

            modelBuilder.Entity<AuthResource>(entity =>
            {
                entity.HasKey(e => e.ResourceId);

                entity.ToTable("authResources");

                entity.HasIndex(e => e.ResourceCode, "ResourceCode_must_be_unique")
                    .IsUnique();

                entity.Property(e => e.ResourceId).HasColumnName("resourceId");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.CreatedUserId).HasColumnName("createdUserId");

                entity.Property(e => e.OrderIndex).HasColumnName("orderIndex");

                entity.Property(e => e.ParentResourceCode)
                    .HasMaxLength(250)
                    .HasColumnName("parentResourceCode");

                entity.Property(e => e.ResourceCode)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("resourceCode");

                entity.Property(e => e.ResourceName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("resourceName");

                entity.Property(e => e.ResourceType).HasColumnName("resourceType");

                entity.Property(e => e.SeoDescription)
                    .HasMaxLength(175)
                    .HasColumnName("seoDescription")
                    .HasComment("This tag provides a short description of the page.");

                entity.Property(e => e.SeoKeywords)
                    .HasMaxLength(75)
                    .HasColumnName("seoKeywords");

                entity.Property(e => e.SeoTitle)
                    .HasMaxLength(75)
                    .HasColumnName("seoTitle")
                    .HasComment("While technically not a meta tag, this tag is often used together with the \"description\". The contents of this tag are generally shown as the title in search results (and of course in the user's browser).");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.TableName)
                    .HasMaxLength(100)
                    .HasColumnName("tableName");

                entity.Property(e => e.TenantId).HasColumnName("tenantId");

                entity.Property(e => e.UpdatedDate).HasColumnName("updatedDate");

                entity.Property(e => e.UpdatedUserId).HasColumnName("updatedUserId");
            });

            modelBuilder.Entity<AuthTemplate>(entity =>
            {
                entity.ToTable("authTemplate");

                entity.Property(e => e.AuthTemplateId).HasColumnName("authTemplateId");

                entity.Property(e => e.IsDefault).HasColumnName("isDefault");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TemplateDefaultPage).HasColumnName("templateDefaultPage");

                entity.Property(e => e.TemplateName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("templateName");

                entity.Property(e => e.TemplateType).HasColumnName("templateType");

                entity.Property(e => e.TenantId).HasColumnName("tenantId");
            });

            modelBuilder.Entity<AuthTemplateDetail>(entity =>
            {
                entity.ToTable("authTemplateDetail");

                entity.HasIndex(e => e.AuthTemplateId, "IX_AuthTemplateDetail_authTemplateDetail");

                entity.Property(e => e.AuthTemplateDetailId).HasColumnName("authTemplateDetailId");

                entity.Property(e => e.ActionId).HasColumnName("actionId");

                entity.Property(e => e.AuthTemplateId).HasColumnName("authTemplateId");

                entity.Property(e => e.ResourceId).HasColumnName("resourceId");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TenantId).HasColumnName("tenantId");

                entity.HasOne(d => d.AuthTemplate)
                    .WithMany(p => p.AuthTemplateDetails)
                    .HasForeignKey(d => d.AuthTemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("authTemplateDetail.authTemplateId.fkey");
            });

            modelBuilder.Entity<AuthUserRight>(entity =>
            {
                entity.HasKey(e => e.RightId);

                entity.ToTable("authUserRights");

                entity.HasIndex(e => e.ActionId, "IX_AuthUserRights_ActionId");

                entity.HasIndex(e => e.UserId, "IX_AuthUserRights_UserId");

                entity.Property(e => e.RightId).HasColumnName("rightId");

                entity.Property(e => e.ActionId).HasColumnName("actionId");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.CreatedUserId).HasColumnName("createdUserId");

                entity.Property(e => e.GroupActionId).HasColumnName("groupActionId");

                entity.Property(e => e.RecordType).HasColumnName("recordType");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TenantId).HasColumnName("tenantId");

                entity.Property(e => e.UpdatedDate).HasColumnName("updatedDate");

                entity.Property(e => e.UpdatedUserId).HasColumnName("updatedUserId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.AuthUserRights)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("authUserRights_actionId_fkey");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AuthUserRights)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("authUserRights_userId_fkey");
            });

            modelBuilder.Entity<CommunicationDefinition>(entity =>
            {
                entity.HasKey(e => e.CommDefinitionId);

                entity.ToTable("communicationDefinitions");

                entity.Property(e => e.CommDefinitionId).HasColumnName("commDefinitionId");

                entity.Property(e => e.CommDefinitionName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("commDefinitionName");

                entity.Property(e => e.CommDefinitionType).HasColumnName("commDefinitionType");

                entity.Property(e => e.CompanyId).HasColumnName("companyId");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.CreatedUserId).HasColumnName("createdUserId");

                entity.Property(e => e.EmailPassword)
                    .HasMaxLength(100)
                    .HasColumnName("emailPassword");

                entity.Property(e => e.EmailPort)
                    .HasMaxLength(10)
                    .HasColumnName("emailPort");

                entity.Property(e => e.EmailSecurityType).HasColumnName("emailSecurityType");

                entity.Property(e => e.EmailSenderAccount)
                    .HasMaxLength(100)
                    .HasColumnName("emailSenderAccount");

                entity.Property(e => e.EmailSenderName)
                    .HasMaxLength(100)
                    .HasColumnName("emailSenderName");

                entity.Property(e => e.EmailSmtpServer)
                    .HasMaxLength(100)
                    .HasColumnName("emailSmtpServer");

                entity.Property(e => e.EmailUsername)
                    .HasMaxLength(100)
                    .HasColumnName("emailUsername");

                entity.Property(e => e.SmsAuthData)
                    .HasMaxLength(1000)
                    .HasColumnName("smsAuthData");

                entity.Property(e => e.SmsCompanyName)
                    .HasMaxLength(100)
                    .HasColumnName("smsCompanyName");

                entity.Property(e => e.SmsCustomerCode)
                    .HasMaxLength(75)
                    .HasColumnName("smsCustomerCode");

                entity.Property(e => e.SmsEndpointUrl)
                    .HasMaxLength(150)
                    .HasColumnName("smsEndpointUrl");

                entity.Property(e => e.SmsExpectedResponse)
                    .HasMaxLength(250)
                    .HasColumnName("smsExpectedResponse");

                entity.Property(e => e.SmsExpectedStatusCode)
                    .HasMaxLength(6)
                    .HasColumnName("smsExpectedStatusCode");

                entity.Property(e => e.SmsFormData)
                    .HasMaxLength(500)
                    .HasColumnName("smsFormData");

                entity.Property(e => e.SmsPassword)
                    .HasMaxLength(100)
                    .HasColumnName("smsPassword");

                entity.Property(e => e.SmsProviderCode)
                    .HasMaxLength(50)
                    .HasColumnName("smsProviderCode");

                entity.Property(e => e.SmsSenderNumber)
                    .HasMaxLength(75)
                    .HasColumnName("smsSenderNumber");

                entity.Property(e => e.SmsUsername)
                    .HasMaxLength(75)
                    .HasColumnName("smsUsername");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.TenantId).HasColumnName("tenantId");

                entity.Property(e => e.Timezone)
                    .HasMaxLength(250)
                    .HasColumnName("timezone");

                entity.Property(e => e.UpdatedDate).HasColumnName("updatedDate");

                entity.Property(e => e.UpdatedUserId).HasColumnName("updatedUserId");
            });

            modelBuilder.Entity<CommunicationTemplate>(entity =>
            {
                entity.HasKey(e => e.CommTemplateId);

                entity.ToTable("communicationTemplates");

                entity.Property(e => e.CommTemplateId).HasColumnName("commTemplateId");

                entity.Property(e => e.CommDefinitionId).HasColumnName("commDefinitionId");

                entity.Property(e => e.CommTemplateName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("commTemplateName");

                entity.Property(e => e.CompanyId).HasColumnName("companyId");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.CreatedUserId).HasColumnName("createdUserId");

                entity.Property(e => e.EmailBccs)
                    .HasMaxLength(250)
                    .HasColumnName("emailBCCs");

                entity.Property(e => e.EmailBody)
                    .HasColumnType("character varying")
                    .HasColumnName("emailBody");

                entity.Property(e => e.EmailCcs)
                    .HasMaxLength(250)
                    .HasColumnName("emailCCs");

                entity.Property(e => e.EmailIsBodyHtml).HasColumnName("emailIsBodyHtml");

                entity.Property(e => e.EmailPriority).HasColumnName("emailPriority");

                entity.Property(e => e.EmailRecipients)
                    .HasMaxLength(250)
                    .HasColumnName("emailRecipients");

                entity.Property(e => e.EmailSenderName)
                    .HasMaxLength(100)
                    .HasColumnName("emailSenderName");

                entity.Property(e => e.EmailSubject)
                    .HasMaxLength(250)
                    .HasColumnName("emailSubject");

                entity.Property(e => e.RequestConditions).HasColumnName("requestConditions");

                entity.Property(e => e.RequestType)
                    .HasMaxLength(1000)
                    .HasColumnName("requestType");

                entity.Property(e => e.ResponseType)
                    .HasMaxLength(1000)
                    .HasColumnName("responseType");

                entity.Property(e => e.ServiceUrls).HasColumnName("serviceUrls");

                entity.Property(e => e.SmsBody)
                    .HasColumnType("character varying")
                    .HasColumnName("smsBody");

                entity.Property(e => e.SmsRecipients)
                    .HasMaxLength(250)
                    .HasColumnName("smsRecipients");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.TenantId).HasColumnName("tenantId");

                entity.Property(e => e.Timezone)
                    .HasMaxLength(250)
                    .HasColumnName("timezone");

                entity.Property(e => e.UpdatedDate).HasColumnName("updatedDate");

                entity.Property(e => e.UpdatedUserId).HasColumnName("updatedUserId");
            });

            modelBuilder.Entity<CoreCompany>(entity =>
            {
                entity.HasKey(e => e.CompanyId);

                entity.ToTable("coreCompany");

                entity.HasIndex(e => e.CompanyName, "CompanyName_must_be_unique")
                    .IsUnique();

                entity.Property(e => e.CompanyId).HasColumnName("companyId");

                entity.Property(e => e.Address)
                    .HasMaxLength(250)
                    .HasColumnName("address");

                entity.Property(e => e.BillingAddress)
                    .HasMaxLength(250)
                    .HasColumnName("billingAddress");

                entity.Property(e => e.CityId).HasColumnName("cityId");

                entity.Property(e => e.CompanyLegalTitle)
                    .HasMaxLength(150)
                    .HasColumnName("companyLegalTitle");

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("companyName");

                entity.Property(e => e.ContactPerson)
                    .HasMaxLength(75)
                    .HasColumnName("contactPerson");

                entity.Property(e => e.ContactPersonEmail)
                    .HasMaxLength(75)
                    .HasColumnName("contactPersonEmail");

                entity.Property(e => e.ContactPersonTelephone)
                    .HasMaxLength(20)
                    .HasColumnName("contactPersonTelephone");

                entity.Property(e => e.ContactPersonTitle)
                    .HasMaxLength(50)
                    .HasColumnName("contactPersonTitle");

                entity.Property(e => e.CountryId).HasColumnName("countryId");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.CreatedUserId).HasColumnName("createdUserId");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.Note).HasColumnName("note");

                entity.Property(e => e.NumberOfStaff).HasColumnName("numberOfStaff");

                entity.Property(e => e.SectorId).HasColumnName("sectorId");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.TaxNumber)
                    .HasMaxLength(20)
                    .HasColumnName("taxNumber");

                entity.Property(e => e.TaxOffice)
                    .HasMaxLength(50)
                    .HasColumnName("taxOffice");

                entity.Property(e => e.Telephone)
                    .HasMaxLength(20)
                    .HasColumnName("telephone");

                entity.Property(e => e.TenantId).HasColumnName("tenantId");

                entity.Property(e => e.TownId).HasColumnName("townId");

                entity.Property(e => e.UpdatedDate).HasColumnName("updatedDate");

                entity.Property(e => e.UpdatedUserId).HasColumnName("updatedUserId");

                entity.Property(e => e.Website)
                    .HasMaxLength(100)
                    .HasColumnName("website");
            });

            modelBuilder.Entity<CoreDepartment>(entity =>
            {
                entity.HasKey(e => e.DepartmentId);

                entity.ToTable("coreDepartment");

                entity.HasIndex(e => e.CompanyId, "IX_coreDepartment_companyId");

                entity.HasIndex(e => e.DepHeadUserId, "IX_coreDepartment_depHeadUserId");

                entity.Property(e => e.DepartmentId).HasColumnName("departmentId");

                entity.Property(e => e.CompanyId).HasColumnName("companyId");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.CreatedUserId).HasColumnName("createdUserId");

                entity.Property(e => e.DepHeadUserId).HasColumnName("depHeadUserId");

                entity.Property(e => e.DepartmentName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("departmentName");

                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .HasColumnName("description");

                entity.Property(e => e.ParentDepartmentId).HasColumnName("parentDepartmentId");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.TenantId).HasColumnName("tenantId");

                entity.Property(e => e.UpdatedDate).HasColumnName("updatedDate");

                entity.Property(e => e.UpdatedUserId).HasColumnName("updatedUserId");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CoreDepartments)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("coredepartment_corecompany_fk");

                entity.HasOne(d => d.DepHeadUser)
                    .WithMany(p => p.CoreDepartments)
                    .HasForeignKey(d => d.DepHeadUserId)
                    .HasConstraintName("coredepartment_coreuser_depheaduserid_fk");
            });

            modelBuilder.Entity<CoreParameter>(entity =>
            {
                entity.HasKey(e => e.ParameterId)
                    .HasName("PK_parameters");

                entity.ToTable("coreParameters");

                entity.HasIndex(e => new { e.TenantId, e.KeyCode, e.ParentValue, e.Value, e.Status }, "KEYCODE_AND_VALUE_PAIR_MUST_BE_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ParameterId).HasColumnName("parameterId");

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .HasColumnName("description");

                entity.Property(e => e.KeyCode)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("keyCode");

                entity.Property(e => e.OrderIndex).HasColumnName("orderIndex");

                entity.Property(e => e.ParentValue).HasColumnName("parentValue");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TenantId).HasColumnName("tenantId");

                entity.Property(e => e.Translations)
                    .HasColumnType("json")
                    .HasColumnName("translations");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("value");
            });

            modelBuilder.Entity<CoreUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_users");

                entity.ToTable("coreUsers");

                entity.HasIndex(e => e.Email, "EMAIL_MUST_BE_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasColumnName("address");

                entity.Property(e => e.BirthDate)
                    .HasColumnType("date")
                    .HasColumnName("birthDate");

                entity.Property(e => e.CityId).HasColumnName("cityId");

                entity.Property(e => e.CompanyId).HasColumnName("companyId");

                entity.Property(e => e.CountyId).HasColumnName("countyId");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.CreatedUserId).HasColumnName("createdUserId");

                entity.Property(e => e.DepartmentId).HasColumnName("departmentId");

                entity.Property(e => e.Email)
                    .HasMaxLength(80)
                    .HasColumnName("email");

                entity.Property(e => e.ForgotPasswordExpiration).HasColumnName("forgotPasswordExpiration");

                entity.Property(e => e.ForgotPasswordKey).HasColumnName("forgotPasswordKey");

                entity.Property(e => e.Gender).HasColumnName("gender");

                entity.Property(e => e.GeneralUser).HasColumnName("generalUser");

                entity.Property(e => e.IbanNumber)
                    .HasMaxLength(33)
                    .HasColumnName("ibanNumber");

                entity.Property(e => e.IdentificationNo)
                    .HasMaxLength(15)
                    .HasColumnName("identificationNo");

                entity.Property(e => e.IdentityType).HasColumnName("identityType");

                entity.Property(e => e.IsAuthorized).HasColumnName("isAuthorized");

                entity.Property(e => e.MaritalStatus).HasColumnName("maritalStatus");

                entity.Property(e => e.MarriageDate)
                    .HasColumnType("date")
                    .HasColumnName("marriageDate");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasMaxLength(64)
                    .HasColumnName("password");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(254)
                    .HasColumnName("phoneNumber");

                entity.Property(e => e.RegistrationNumber)
                    .HasMaxLength(20)
                    .HasColumnName("registrationNumber");

                entity.Property(e => e.RelatedUserId).HasColumnName("relatedUserId");

                entity.Property(e => e.RelationType).HasColumnName("relationType");

                entity.Property(e => e.RoleId).HasColumnName("roleId");

                entity.Property(e => e.ShouldChangePassword).HasColumnName("shouldChangePassword");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("surname");

                entity.Property(e => e.TempPassword)
                    .HasMaxLength(64)
                    .HasColumnName("tempPassword");

                entity.Property(e => e.TenantId).HasColumnName("tenantId");

                entity.Property(e => e.UpdatedDate).HasColumnName("updatedDate");

                entity.Property(e => e.UpdatedUserId).HasColumnName("updatedUserId");

                entity.Property(e => e.UserTitle).HasColumnName("userTitle");

                entity.Property(e => e.VerificationKey).HasColumnName("verificationKey");

                entity.Property(e => e.VerificationKeyExpiration).HasColumnName("verificationKeyExpiration");

                entity.Property(e => e.WorkEndTime)
                    .HasColumnType("date")
                    .HasColumnName("workEndTime");

                entity.Property(e => e.WorkStartTime)
                    .HasColumnType("date")
                    .HasColumnName("workStartTime");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notification");

                entity.HasIndex(e => e.NotificationSettingsId, "IX_notification_notificationSettingsId");

                entity.Property(e => e.NotificationId).HasColumnName("notificationId");

                entity.Property(e => e.CreatedDate).HasColumnName("createdDate");

                entity.Property(e => e.CreatedUserId).HasColumnName("createdUserId");

                entity.Property(e => e.DeliveryDate).HasColumnName("deliveryDate");

                entity.Property(e => e.NotificationSettingsId).HasColumnName("notificationSettingsId");

                entity.Property(e => e.SendDate).HasColumnName("sendDate");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TenantId).HasColumnName("tenantId");

                entity.Property(e => e.UpdatedDate).HasColumnName("updatedDate");

                entity.Property(e => e.UpdatedUserId).HasColumnName("updatedUserId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.NotificationSettings)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.NotificationSettingsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("notification_settings_fk");
            });

            modelBuilder.Entity<NotificationSetting>(entity =>
            {
                entity.HasKey(e => e.NotificationSettingsId);

                entity.ToTable("notificationSettings");

                entity.Property(e => e.NotificationSettingsId).HasColumnName("notificationSettingsId");

                entity.Property(e => e.AgeRange)
                    .HasMaxLength(20)
                    .HasColumnName("ageRange");

                entity.Property(e => e.City).HasColumnName("city");

                entity.Property(e => e.CompanyId).HasColumnName("companyId");

                entity.Property(e => e.CreatedDate).HasColumnName("createdDate");

                entity.Property(e => e.CreatedUserId).HasColumnName("createdUserId");

                entity.Property(e => e.Data)
                    .HasColumnType("json")
                    .HasColumnName("data");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .HasColumnName("description");

                entity.Property(e => e.Gender).HasColumnName("gender");

                entity.Property(e => e.Message)
                    .HasMaxLength(254)
                    .HasColumnName("message");

                entity.Property(e => e.NotificationType).HasColumnName("notificationType");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Subtitle)
                    .HasMaxLength(60)
                    .HasColumnName("subtitle");

                entity.Property(e => e.TenantId).HasColumnName("tenantId");

                entity.Property(e => e.Title)
                    .HasMaxLength(60)
                    .HasColumnName("title");

                entity.Property(e => e.UpdatedDate).HasColumnName("updatedDate");

                entity.Property(e => e.UpdatedUserId).HasColumnName("updatedUserId");

                entity.Property(e => e.UserId).HasColumnName("userId");
            });

            modelBuilder.Entity<SampleEmployee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);

                entity.ToTable("sampleEmployee");

                entity.HasIndex(e => e.EmployeeId, "IX_sampleEmployee_employeeId");

                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

                entity.Property(e => e.CityId).HasColumnName("cityId");

                entity.Property(e => e.CompanyId).HasColumnName("companyId");

                entity.Property(e => e.CountyId).HasColumnName("countyId");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(80)
                    .HasColumnName("email");

                entity.Property(e => e.EmployeeName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("employeeName");

                entity.Property(e => e.EmployeeSurname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("employeeSurname");

                entity.Property(e => e.EmployeeTitle).HasColumnName("employeeTitle");

                entity.Property(e => e.Gender).HasColumnName("gender");

                entity.Property(e => e.IbanNumber).HasColumnName("ibanNumber");

                entity.Property(e => e.Note).HasColumnName("note");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnName("password");

                entity.Property(e => e.PhoneNumber).HasColumnName("phoneNumber");

                entity.Property(e => e.Picture).HasColumnName("picture");

                entity.Property(e => e.Salary).HasColumnName("salary");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.TaxNumber)
                    .IsRequired()
                    .HasColumnName("taxNumber");

                entity.Property(e => e.WorkStartDate).HasColumnName("workStartDate");
            });

            modelBuilder.Entity<SampleEmployeeTask>(entity =>
            {
                entity.HasKey(e => e.EmployeeTaskId);

                entity.ToTable("sampleEmployeeTask");

                entity.Property(e => e.EmployeeTaskId).HasColumnName("employeeTaskId");

                entity.Property(e => e.DueDate).HasColumnName("dueDate");

                entity.Property(e => e.EmployeeId).HasColumnName("employeeId");

                entity.Property(e => e.EmployeeTaskDescription).HasColumnName("employeeTaskDescription");

                entity.Property(e => e.EmployeeTaskName).HasColumnName("employeeTaskName");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.TaskTags).HasColumnName("taskTags");
            });

            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.ToTable("tenants");

                entity.HasIndex(e => e.ParentTenantId, "IX_tenants_parentTenantId");

                entity.Property(e => e.TenantId).HasColumnName("tenantId");

                entity.Property(e => e.Address).HasColumnName("address");

                entity.Property(e => e.CityId).HasColumnName("cityId");

                entity.Property(e => e.CountryId).HasColumnName("countryId");

                entity.Property(e => e.CreatedDate).HasColumnName("createdDate");

                entity.Property(e => e.CreatedUserId).HasColumnName("createdUserId");

                entity.Property(e => e.Email).HasColumnName("email");

                entity.Property(e => e.IsDefault).HasColumnName("isDefault");

                entity.Property(e => e.Note)
                    .HasMaxLength(500)
                    .HasColumnName("note");

                entity.Property(e => e.ParentTenantId).HasColumnName("parentTenantId");

                entity.Property(e => e.PhoneNumber).HasColumnName("phoneNumber");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TaxNumber)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("taxNumber");

                entity.Property(e => e.TaxOffice)
                    .HasMaxLength(50)
                    .HasColumnName("taxOffice");

                entity.Property(e => e.TenantName).HasColumnName("tenantName");

                entity.Property(e => e.TenantType).HasColumnName("tenantType");

                entity.Property(e => e.UpdatedDate).HasColumnName("updatedDate");

                entity.Property(e => e.UpdatedUserId).HasColumnName("updatedUserId");

                entity.Property(e => e.Website).HasColumnName("website");

                entity.HasOne(d => d.ParentTenant)
                    .WithMany(p => p.InverseParentTenant)
                    .HasForeignKey(d => d.ParentTenantId);
            });

            modelBuilder.Entity<TransactionLog>(entity =>
            {
                entity.HasKey(e => e.LogId)
                    .HasName("transactionLogs_pkey");

                entity.ToTable("transactionLogs");

                entity.Property(e => e.LogId).HasColumnName("logId");

                entity.Property(e => e.CurrentClaims)
                    .HasColumnType("json")
                    .HasColumnName("currentClaims");

                entity.Property(e => e.LogDateBegin)
                    .HasColumnName("logDateBegin")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.LogDateEnd).HasColumnName("logDateEnd");

                entity.Property(e => e.LogType).HasColumnName("logType");

                entity.Property(e => e.Request)
                    .HasColumnType("json")
                    .HasColumnName("request");

                entity.Property(e => e.Response)
                    .HasColumnType("json")
                    .HasColumnName("response");

                entity.Property(e => e.ServiceUrl)
                    .IsRequired()
                    .HasMaxLength(254)
                    .HasColumnName("serviceUrl");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StatusCode).HasColumnName("statusCode");

                entity.Property(e => e.TenantId).HasColumnName("tenantId");

                entity.Property(e => e.UserId).HasColumnName("userId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}