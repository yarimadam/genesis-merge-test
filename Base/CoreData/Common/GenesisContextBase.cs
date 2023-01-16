using System;
using CoreType.DBModels;
using CoreType.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreData.Common
{
    public abstract class GenesisContextBase : ContextBase
    {
        public GenesisContextBase()
        {
        }

        public GenesisContextBase(DbContextOptions options)
            : base(options)
        {
        }

        public GenesisContextBase(SessionContext session) : base(session)
        {
        }

        public GenesisContextBase(DbContextOptions options, SessionContext session)
            : base(options, session)
        {
        }

        public virtual DbSet<AuthActions> AuthActions { get; set; }
        public virtual DbSet<AuthResources> AuthResources { get; set; }
        public virtual DbSet<AuthTemplate> AuthTemplate { get; set; }
        public virtual DbSet<AuthTemplateDetail> AuthTemplateDetail { get; set; }
        public virtual DbSet<AuthUserRights> AuthUserRights { get; set; }
        public virtual DbSet<CommunicationDefinitions> CommunicationDefinitions { get; set; }
        public virtual DbSet<CommunicationTemplates> CommunicationTemplates { get; set; }
        public virtual DbSet<CoreCompany> CoreCompany { get; set; }
        public virtual DbSet<CoreDepartment> CoreDepartment { get; set; }
        public virtual DbSet<CoreParameters> CoreParameters { get; set; }
        public virtual DbSet<CoreUsers> CoreUsers { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<NotificationSettings> NotificationSettings { get; set; }
        public virtual DbSet<TransactionLogs> TransactionLogs { get; set; }
        public virtual DbSet<Tenant> Tenants { get; set; }

        // protected string NUMERIC_DEFAULT_VALUE_1;
        // protected string DATE_COLUMN_TYPE;
        protected string DATE_DEFAULT_VALUE;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasAnnotation("ProductVersion", "2.2.1-servicing-10028");

            SetConfigurationVariables();

            #region Management Models

            modelBuilder.ApplyConfiguration<AuthActions>(AuthActionsConfiguration);
            modelBuilder.ApplyConfiguration<AuthResources>(AuthResoureConfiguration);
            modelBuilder.ApplyConfiguration<AuthTemplate>(AuthTemplateConfiguration);
            modelBuilder.ApplyConfiguration<AuthTemplateDetail>(AuthTemplateDetailConfiguration);
            modelBuilder.ApplyConfiguration<AuthUserRights>(AuthUserRightsConfiguration);
            modelBuilder.ApplyConfiguration<CommunicationDefinitions>(CommunicationDefinitionsConfiguration);
            modelBuilder.ApplyConfiguration<CommunicationTemplates>(CommunicationTemplatesConfiguration);
            modelBuilder.ApplyConfiguration<CoreCompany>(CoreCompanyConfiguration);
            modelBuilder.ApplyConfiguration<CoreDepartment>(CoreDepartmentConfiguration);
            modelBuilder.ApplyConfiguration<CoreParameters>(CoreParametersConfiguration);
            modelBuilder.ApplyConfiguration<CoreUsers>(CoreUsersConfiguration);
            modelBuilder.ApplyConfiguration<Notification>(NotificationConfiguration);
            modelBuilder.ApplyConfiguration<NotificationSettings>(NotificationSettingsConfiguration);
            modelBuilder.ApplyConfiguration<TransactionLogs>(TransactionLogsConfiguration);
            modelBuilder.ApplyConfiguration<Tenant>(TenantConfiguration);

            #endregion

        }

        private void SetConfigurationVariables()
        {
            switch (this.GetDatabaseType())
            {
                case DatabaseType.PostgreSQL:
                    // DATE_COLUMN_TYPE = "date";
                    DATE_DEFAULT_VALUE = "now()";
                    // NUMERIC_DEFAULT_VALUE_1 = "'1'::smallint";
                    break;
                case DatabaseType.MSSQL:
                    // DATE_COLUMN_TYPE = "date";
                    DATE_DEFAULT_VALUE = "getdate()";
                    // NUMERIC_DEFAULT_VALUE_1 = "1";
                    break;
                case DatabaseType.MySQL:
                    // DATE_COLUMN_TYPE = "timestamp";
                    DATE_DEFAULT_VALUE = "CURRENT_TIMESTAMP(6)";
                    // NUMERIC_DEFAULT_VALUE_1 = "1";
                    break;
                case DatabaseType.Oracle:
                    // DATE_COLUMN_TYPE = "TIMESTAMP";
                    DATE_DEFAULT_VALUE = "current_date";
                    // NUMERIC_DEFAULT_VALUE_1 = "1";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void AuthActionsConfiguration(EntityTypeBuilder<AuthActions> builder)
        {
            builder.HasKey(e => e.ActionId);

            builder.ToTable("authActions");

            builder.HasIndex(e => new { e.ResourceId, e.ActionType })
                .HasName("authActions_resourceId_and_actionType_must_be_unique") // ORACLE -> IX_AuthActions_resourceId_actionType
                .IsUnique();

            builder.HasIndex(e => e.ResourceId)
                .HasName("IX_AuthActions_resourceId");

            builder.Property(e => e.TenantId)
                .HasColumnName("tenantId")
                .HasDefaultValueSql("0");

            builder.Property(e => e.ActionId)
                .HasColumnName("actionId");
            // .ValueGeneratedOnAdd(); // ORACLE

            builder.Property(e => e.ActionType)
                .HasColumnName("actionType");

            builder.Property(e => e.CreatedDate)
                .HasColumnName("createdDate")
                // .ValueGeneratedNever() // ORACLE
                .HasDefaultValueSql(DATE_DEFAULT_VALUE);

            builder.Property(e => e.CreatedUserId)
                .HasColumnName("createdUserId");

            builder.Property(e => e.OrderIndex)
                .HasColumnName("orderIndex");

            builder.Property(e => e.ResourceId)
                .HasColumnName("resourceId");

            builder.Property(e => e.Status)
                .HasColumnName("status")
                // .ValueGeneratedNever() // ORACLE
                .HasDefaultValueSql("1");

            builder.Property(e => e.UpdatedDate)
                .HasColumnName("updatedDate");

            builder.Property(e => e.UpdatedUserId)
                .HasColumnName("updatedUserId");

            builder.HasOne(d => d.Resource)
                .WithMany(p => p.AuthActions)
                .HasForeignKey(d => d.ResourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("authActions_resourceId_fkey");
        }

        protected virtual void AuthResoureConfiguration(EntityTypeBuilder<AuthResources> builder)
        {
            builder.HasKey(e => e.ResourceId);

            builder.ToTable("authResources");

            builder.HasIndex(e => e.ResourceCode)
                .HasName("ResourceCode_must_be_unique") // ORACLE -> IX_UQ_AuthResources_ResourceCode
                .IsUnique();

            builder.Property(e => e.ResourceId)
                .HasColumnName("resourceId");
            // .ValueGeneratedOnAdd(); // ORACLE

            builder.Property(e => e.CreatedDate)
                .HasColumnName("createdDate")
                //  .ValueGeneratedNever() // ORACLE
                .HasDefaultValueSql(DATE_DEFAULT_VALUE);

            builder.Property(e => e.CreatedUserId)
                .HasColumnName("createdUserId");

            builder.Property(e => e.OrderIndex)
                .HasColumnName("orderIndex");

            builder.Property(e => e.ParentResourceCode)
                .HasColumnName("parentResourceCode")
                .HasMaxLength(250);

            builder.Property(e => e.TenantId)
                .HasColumnName("tenantId")
                .HasDefaultValueSql("0");

            builder.Property(e => e.ResourceCode)
                .HasColumnName("resourceCode")
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(e => e.ResourceName)
                .HasColumnName("resourceName")
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(e => e.ResourceType)
                .HasColumnName("resourceType");

            builder.Property(e => e.SeoDescription)
                .HasColumnName("seoDescription")
                .HasMaxLength(175)
                .HasComment("This tag provides a short description of the page.");

            builder.Property(e => e.SeoKeywords)
                .HasColumnName("seoKeywords")
                .HasMaxLength(75);

            builder.Property(e => e.SeoTitle)
                .HasColumnName("seoTitle")
                .HasMaxLength(75)
                .HasComment(
                    "While technically not a meta tag, this tag is often used together with the \"description\". The contents of this tag are generally shown as the title in search results (and of course in the user's browser).");

            builder.Property(e => e.Status)
                .HasColumnName("status")
                // .ValueGeneratedNever() // ORACLE
                .HasDefaultValueSql("1");

            builder.Property(e => e.TableName)
                .HasColumnName("tableName")
                .HasMaxLength(100);

            builder.Property(e => e.UpdatedDate)
                .HasColumnName("updatedDate");

            builder.Property(e => e.UpdatedUserId)
                .HasColumnName("updatedUserId");
        }

        protected virtual void AuthTemplateConfiguration(EntityTypeBuilder<AuthTemplate> builder)
        {
            builder.ToTable("authTemplate");

            builder.Property(e => e.TenantId)
                .HasColumnName("tenantId")
                .HasDefaultValueSql("0");

            builder.Property(e => e.AuthTemplateId)
                .HasColumnName("authTemplateId");
            // .ValueGeneratedOnAdd(); // ORACLE

            builder.Property(e => e.TemplateDefaultPage)
                .HasColumnName("templateDefaultPage");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.TemplateName)
                .HasColumnName("templateName")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.TemplateType)
                .HasColumnName("templateType");

            builder.Property(e => e.IsDefault)
                .HasColumnName("isDefault")
                .HasDefaultValue(false);
        }

        protected virtual void AuthTemplateDetailConfiguration(EntityTypeBuilder<AuthTemplateDetail> builder)
        {
            builder.ToTable("authTemplateDetail");

            builder
                .HasIndex(e => e.AuthTemplateId)
                .HasName("IX_AuthTemplateDetail_authTemplateDetail");

            builder.Property(e => e.TenantId)
                .HasColumnName("tenantId")
                .HasDefaultValueSql("0");

            builder.Property(e => e.AuthTemplateDetailId)
                .HasColumnName("authTemplateDetailId");
            // .ValueGeneratedOnAdd(); // ORACLE

            builder.Property(e => e.ActionId)
                .HasColumnName("actionId");

            builder.Property(e => e.AuthTemplateId)
                .HasColumnName("authTemplateId");

            builder.Property(e => e.ResourceId)
                .HasColumnName("resourceId");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.HasOne(d => d.AuthTemplate)
                .WithMany(p => p.AuthTemplateDetail)
                .HasForeignKey(d => d.AuthTemplateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("authTemplateDetail.authTemplateId.fkey"); // ORACLE -> templateDetail.templateId.fkey
        }

        protected virtual void AuthUserRightsConfiguration(EntityTypeBuilder<AuthUserRights> builder)
        {
            builder.HasKey(e => e.RightId);

            builder.ToTable("authUserRights");

            builder.Property(e => e.TenantId)
                .HasColumnName("tenantId")
                .HasDefaultValueSql("0");

            builder.HasIndex(e => e.ActionId)
                .HasName("IX_AuthUserRights_ActionId");

            builder.HasIndex(e => e.UserId)
                .HasName("IX_AuthUserRights_UserId");

            builder.Property(e => e.RightId)
                .HasColumnName("rightId");
            // .ValueGeneratedOnAdd(); // ORACLE

            builder.Property(e => e.ActionId)
                .HasColumnName("actionId");

            builder.Property(e => e.CreatedDate)
                .HasColumnName("createdDate")
                // .ValueGeneratedNever() // ORACLE
                .HasDefaultValueSql(DATE_DEFAULT_VALUE);

            builder.Property(e => e.CreatedUserId)
                .HasColumnName("createdUserId");

            builder.Property(e => e.GroupActionId)
                .HasColumnName("groupActionId");

            builder.Property(e => e.RecordType)
                .HasColumnName("recordType");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.UpdatedDate)
                .HasColumnName("updatedDate");

            builder.Property(e => e.UpdatedUserId)
                .HasColumnName("updatedUserId");

            builder.Property(e => e.UserId)
                .HasColumnName("userId");

            builder.HasOne(d => d.Action)
                .WithMany(p => p.AuthUserRights)
                .HasForeignKey(d => d.ActionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("authUserRights_actionId_fkey");

            builder.HasOne(d => d.User)
                .WithMany(p => p.AuthUserRights)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("authUserRights_userId_fkey");
        }

        protected virtual void CommunicationDefinitionsConfiguration(EntityTypeBuilder<CommunicationDefinitions> builder)
        {
            builder.HasKey(e => e.CommDefinitionId);

            builder.ToTable("communicationDefinitions");

            builder.Property(e => e.TenantId)
                .HasColumnName("tenantId")
                .HasDefaultValueSql("0");

            builder.Property(e => e.CommDefinitionId)
                .HasColumnName("commDefinitionId");
            // .ValueGeneratedOnAdd(); // ORACLE

            builder.Property(e => e.CommDefinitionName)
                .HasColumnName("commDefinitionName")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.CommDefinitionType)
                .HasColumnName("commDefinitionType");

            builder.Property(e => e.CompanyId)
                .HasColumnName("companyId");

            builder.Property(e => e.EmailPassword)
                .HasColumnName("emailPassword")
                .HasMaxLength(100);

            builder.Property(e => e.EmailPort)
                .HasColumnName("emailPort")
                .HasMaxLength(10);

            builder.Property(e => e.EmailSecurityType)
                .HasColumnName("emailSecurityType");

            builder.Property(e => e.EmailSenderAccount)
                .HasColumnName("emailSenderAccount")
                .HasMaxLength(100);

            builder.Property(e => e.EmailSenderName)
                .HasColumnName("emailSenderName")
                .HasMaxLength(100);

            builder.Property(e => e.EmailSmtpServer)
                .HasColumnName("emailSmtpServer")
                .HasMaxLength(100);

            builder.Property(e => e.EmailUsername)
                .HasColumnName("emailUsername")
                .HasMaxLength(100);

            builder.Property(e => e.CreatedDate)
                .HasColumnName("createdDate")
                // .ValueGeneratedNever(); // ORACLE
                .HasDefaultValueSql(DATE_DEFAULT_VALUE);

            builder.Property(e => e.CreatedUserId)
                .HasColumnName("createdUserId");

            builder.Property(e => e.UpdatedDate)
                .HasColumnName("updatedDate");

            builder.Property(e => e.UpdatedUserId)
                .HasColumnName("updatedUserId");

            builder.Property(e => e.SmsAuthData)
                .HasColumnName("smsAuthData")
                .HasMaxLength(1000);

            builder.Property(e => e.SmsCompanyName)
                .HasColumnName("smsCompanyName")
                .HasMaxLength(100);

            builder.Property(e => e.SmsCustomerCode)
                .HasColumnName("smsCustomerCode")
                .HasMaxLength(75);

            builder.Property(e => e.SmsEndpointUrl)
                .HasColumnName("smsEndpointUrl")
                .HasMaxLength(150);

            builder.Property(e => e.SmsExpectedResponse)
                .HasColumnName("smsExpectedResponse")
                .HasMaxLength(250);

            builder.Property(e => e.SmsExpectedStatusCode)
                .HasColumnName("smsExpectedStatusCode")
                .HasMaxLength(6);

            builder.Property(e => e.SmsFormData)
                .HasColumnName("smsFormData")
                .HasMaxLength(500);

            builder.Property(e => e.SmsPassword)
                .HasColumnName("smsPassword")
                .HasMaxLength(100);

            builder.Property(e => e.SmsProviderCode)
                .HasColumnName("smsProviderCode")
                .HasMaxLength(50);

            builder.Property(e => e.SmsSenderNumber)
                .HasColumnName("smsSenderNumber")
                .HasMaxLength(75);

            builder.Property(e => e.SmsUsername)
                .HasColumnName("smsUsername")
                .HasMaxLength(75);

            builder.Property(e => e.Status)
                .HasColumnName("status")
                // .ValueGeneratedNever(); // ORACLE
                // .HasDefaultValueSql(NUMERIC_DEFAULT_VALUE_1); // OLD
                .HasDefaultValue(1); // NEW

            builder.Property(e => e.Timezone)
                .HasColumnName("timezone")
                .HasMaxLength(250);
        }

        protected virtual void CommunicationTemplatesConfiguration(EntityTypeBuilder<CommunicationTemplates> builder)
        {
            builder.HasKey(e => e.CommTemplateId);

            builder.ToTable("communicationTemplates");

            builder.Property(e => e.TenantId)
                .HasColumnName("tenantId")
                .HasDefaultValueSql("0");

            builder.Property(e => e.CommTemplateId)
                .HasColumnName("commTemplateId");
            // .ValueGeneratedOnAdd(); // ORACLE

            builder.Property(e => e.CommDefinitionId)
                .HasColumnName("commDefinitionId");

            builder.Property(e => e.CommTemplateName)
                .HasColumnName("commTemplateName")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.CompanyId)
                .HasColumnName("companyId");

            builder.Property(e => e.EmailBccs)
                .HasColumnName("emailBCCs")
                .HasMaxLength(250);

            builder.Property(e => e.EmailBody)
                .HasColumnName("emailBody");

            builder.Property(e => e.EmailCcs)
                .HasColumnName("emailCCs")
                .HasMaxLength(250);

            builder.Property(e => e.EmailIsBodyHtml)
                .HasColumnName("emailIsBodyHtml");

            builder.Property(e => e.EmailPriority)
                .HasColumnName("emailPriority");

            builder.Property(e => e.EmailRecipients)
                .HasColumnName("emailRecipients")
                .HasMaxLength(250);

            builder.Property(e => e.EmailSenderName)
                .HasColumnName("emailSenderName")
                .HasMaxLength(100);

            builder.Property(e => e.EmailSubject)
                .HasColumnName("emailSubject")
                .HasMaxLength(250);

            builder.Property(e => e.CreatedDate)
                .HasColumnName("createdDate")
                // .ValueGeneratedNever() // ORACLE
                .HasDefaultValueSql(DATE_DEFAULT_VALUE);

            builder.Property(e => e.CreatedUserId)
                .HasColumnName("createdUserId");

            builder.Property(e => e.UpdatedDate)
                .HasColumnName("updatedDate");

            builder.Property(e => e.UpdatedUserId)
                .HasColumnName("updatedUserId");

            builder.Property(e => e.Status)
                .HasColumnName("status")
                // .ValueGeneratedNever() // ORACLE
                // .HasDefaultValueSql(NUMERIC_DEFAULT_VALUE_1); // OLD
                .HasDefaultValue(1); // NEW

            builder.Property(e => e.Timezone)
                .HasColumnName("timezone")
                .HasMaxLength(250);

            builder.Property(e => e.RequestConditions)
                .HasColumnName("requestConditions");

            builder.Property(e => e.RequestType)
                .HasColumnName("requestType")
                .HasMaxLength(1000);

            builder.Property(e => e.ResponseType)
                .HasColumnName("responseType")
                .HasMaxLength(1000);

            builder.Property(e => e.ServiceUrls)
                .HasColumnName("serviceUrls");

            builder.Property(e => e.SmsRecipients)
                .HasColumnName("smsRecipients")
                .HasMaxLength(250);

            builder.Property(e => e.SmsBody)
                .HasColumnName("smsBody");
        }

        protected virtual void CoreCompanyConfiguration(EntityTypeBuilder<CoreCompany> builder)
        {
            builder.HasKey(e => e.CompanyId);

            builder.ToTable("coreCompany");

            builder.HasIndex(e => e.CompanyName)
                .HasName("CompanyName_must_be_unique") // ORACLE -> IX_UQ_CoreCompany_CompanyName
                .IsUnique();

            builder.Property(e => e.CompanyId)
                .HasColumnName("companyId");
            // .ValueGeneratedOnAdd(); // ORACLE

            builder.Property(e => e.TenantId)
                .HasColumnName("tenantId")
                .HasDefaultValueSql("0");

            builder.Property(e => e.Address)
                .HasColumnName("address")
                .HasMaxLength(250);

            builder.Property(e => e.BillingAddress)
                .HasColumnName("billingAddress")
                .HasMaxLength(250);

            builder.Property(e => e.CityId)
                .HasColumnName("cityId");

            builder.Property(e => e.CompanyLegalTitle)
                .HasColumnName("companyLegalTitle")
                .HasMaxLength(150);

            builder.Property(e => e.CompanyName)
                .HasColumnName("companyName")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(e => e.ContactPerson)
                .HasColumnName("contactPerson")
                .HasMaxLength(75);

            builder.Property(e => e.ContactPersonEmail)
                .HasColumnName("contactPersonEmail")
                .HasMaxLength(75);

            builder.Property(e => e.ContactPersonTelephone)
                .HasColumnName("contactPersonTelephone")
                .HasMaxLength(20);

            builder.Property(e => e.ContactPersonTitle)
                .HasColumnName("contactPersonTitle")
                .HasMaxLength(50);

            builder.Property(e => e.CountryId)
                .HasColumnName("countryId");

            builder.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(100);

            builder.Property(e => e.CreatedDate)
                .HasColumnName("createdDate")
                // .ValueGeneratedNever() // ORACLE
                .HasDefaultValueSql(DATE_DEFAULT_VALUE);

            builder.Property(e => e.CreatedUserId)
                .HasColumnName("createdUserId");

            builder.Property(e => e.UpdatedDate)
                .HasColumnName("updatedDate");

            builder.Property(e => e.UpdatedUserId)
                .HasColumnName("updatedUserId");

            builder.Property(e => e.Note)
                .HasColumnName("note");

            builder.Property(e => e.NumberOfStaff)
                .HasColumnName("numberOfStaff");

            builder.Property(e => e.SectorId)
                .HasColumnName("sectorId");

            builder.Property(e => e.Status)
                .HasColumnName("status")
                // .ValueGeneratedNever() // ORACLE
                .HasDefaultValueSql("1");

            builder.Property(e => e.TaxNumber)
                .HasColumnName("taxNumber")
                .HasMaxLength(20);

            builder.Property(e => e.TaxOffice)
                .HasColumnName("taxOffice")
                .HasMaxLength(50);

            builder.Property(e => e.Telephone)
                .HasColumnName("telephone")
                .HasMaxLength(20);

            builder.Property(e => e.TownId)
                .HasColumnName("townId");

            builder.Property(e => e.Website)
                .HasColumnName("website")
                .HasMaxLength(100);
        }

        protected virtual void CoreDepartmentConfiguration(EntityTypeBuilder<CoreDepartment> builder)
        {
            builder.HasKey(e => e.DepartmentId);

            builder.ToTable("coreDepartment");

            builder.Property(e => e.TenantId)
                .HasColumnName("tenantId")
                .HasDefaultValueSql("0");

            builder.Property(e => e.DepartmentId)
                .HasColumnName("departmentId");
            // .ValueGeneratedOnAdd(); // ORACLE

            // builder.HasIndex(e => e.CompanyId).HasName("IX_CoreDepartment_CompanyId"); // ORACLE
            builder.Property(e => e.CompanyId)
                .HasColumnName("companyId");

            // builder.HasIndex(e => e.DepHeadUserId).HasName("IX_CoreDepartment_depHeadUserId"); // ORACLE
            builder.Property(e => e.DepHeadUserId)
                .HasColumnName("depHeadUserId");

            builder.Property(e => e.DepartmentName)
                .HasColumnName("departmentName")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(250);

            builder.Property(e => e.CreatedDate)
                .HasColumnName("createdDate")
                // .ValueGeneratedNever() // ORACLE
                .HasDefaultValueSql(DATE_DEFAULT_VALUE);

            builder.Property(e => e.CreatedUserId)
                .HasColumnName("createdUserId");

            builder.Property(e => e.UpdatedDate)
                .HasColumnName("updatedDate");

            builder.Property(e => e.UpdatedUserId)
                .HasColumnName("updatedUserId");

            builder.Property(e => e.ParentDepartmentId)
                .HasColumnName("parentDepartmentId");

            builder.Property(e => e.Status)
                .HasColumnName("status")
                // .ValueGeneratedNever() // ORACLE
                // .HasDefaultValueSql(NUMERIC_DEFAULT_VALUE_1); // OLD
                .HasDefaultValue(1); // NEW

            builder.HasOne(d => d.Company)
                .WithMany(p => p.CoreDepartment)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("coredepartment_corecompany_fk");

            builder.HasOne(d => d.DepHeadUser)
                .WithMany(p => p.CoreDepartment)
                .HasForeignKey(d => d.DepHeadUserId)
                .HasConstraintName("coredepartment_coreuser_depheaduserid_fk"); // ORACLE -> department_coreuser_userid_fk
        }

        protected virtual void CoreParametersConfiguration(EntityTypeBuilder<CoreParameters> builder)
        {
            builder.HasKey(e => e.ParameterId)
                .HasName("PK_parameters");

            builder.ToTable("coreParameters");

            builder.Property(e => e.TenantId)
                .HasColumnName("tenantId")
                .HasDefaultValueSql("0");

            builder.HasIndex(e => new { e.TenantId, e.KeyCode, e.ParentValue, e.Value, e.Status })
                .HasName("KEYCODE_AND_VALUE_PAIR_MUST_BE_UNIQUE") // ORACLE -> IX_UQ_CoreParameters_KEYCODE_VALUE_STATUS
                .IsUnique();

            builder.Property(e => e.ParameterId)
                .HasColumnName("parameterId");
            // .ValueGeneratedOnAdd(); // ORACLE
            // .UseIdentityColumn(); // MSSQL MYSQL

            builder.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(500);

            builder.Property(e => e.KeyCode)
                .HasColumnName("keyCode")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.OrderIndex)
                .HasColumnName("orderIndex");

            builder.Property(e => e.ParentValue)
                .HasColumnName("parentValue");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.Translations)
                .HasColumnName("translations")
                .HasJsonConversion();

            builder.Property(e => e.Value)
                .HasColumnName("value")
                .HasMaxLength(50)
                .IsRequired();
        }

        protected virtual void CoreUsersConfiguration(EntityTypeBuilder<CoreUsers> builder)
        {
            builder.HasKey(e => e.UserId)
                .HasName("PK_users");

            builder.ToTable("coreUsers");

            builder.HasIndex(e => e.Email)
                .HasName("EMAIL_MUST_BE_UNIQUE")
                .IsUnique();

            builder.Property(e => e.UserId)
                .HasColumnName("userId");
            // .ValueGeneratedOnAdd(); // ORACLE
            // .UseIdentityColumn(); // MSSQL MYSQL

            builder.Property(e => e.Address)
                .HasColumnName("address")
                .HasMaxLength(500);

            builder.Property(e => e.TenantId)
                .HasColumnName("tenantId")
                .HasDefaultValueSql("0");

            builder.Property(e => e.BirthDate)
                .HasColumnName("birthDate")
                .HasColumnType("date");

            builder.Property(e => e.CityId)
                .HasColumnName("cityId");

            builder.Property(e => e.DepartmentId)
                .HasColumnName("departmentId");

            builder.Property(e => e.CompanyId)
                .HasColumnName("companyId");

            builder.Property(e => e.CountyId)
                .HasColumnName("countyId");

            builder.Property(e => e.CreatedDate)
                .HasColumnName("createdDate")
                .HasDefaultValueSql(DATE_DEFAULT_VALUE); // NEW
            // .HasColumnType("timestamp(3) without time zone"); // OLD

            builder.Property(e => e.CreatedUserId)
                .HasColumnName("createdUserId");

            builder.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(80);

            builder.Property(e => e.Gender)
                .HasColumnName("gender");

            builder.Property(e => e.GeneralUser)
                .HasColumnName("generalUser");

            builder.Property(e => e.IbanNumber)
                .HasColumnName("ibanNumber")
                .HasMaxLength(33);

            builder.Property(e => e.IdentificationNo)
                .HasColumnName("identificationNo")
                .HasMaxLength(15);

            builder.Property(e => e.IdentityType)
                .HasColumnName("identityType");

            builder.Property(e => e.IsAuthorized)
                .HasColumnName("isAuthorized");

            builder.Property(e => e.MaritalStatus)
                .HasColumnName("maritalStatus");

            builder.Property(e => e.MarriageDate)
                .HasColumnName("marriageDate")
                .HasColumnType("date");

            builder.Property(e => e.Password)
                .HasColumnName("password")
                .HasMaxLength(64);

            builder.Property(e => e.PhoneNumber)
                .HasColumnName("phoneNumber")
                .HasMaxLength(254);

            builder.Property(e => e.RegistrationNumber)
                .HasColumnName("registrationNumber")
                .HasMaxLength(20);

            builder.Property(e => e.RelatedUserId)
                .HasColumnName("relatedUserId");

            builder.Property(e => e.RelationType)
                .HasColumnName("relationType");

            builder.Property(e => e.RoleId)
                .HasColumnName("roleId");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.TempPassword)
                .HasColumnName("tempPassword")
                .HasMaxLength(64);

            builder.Property(e => e.UpdatedDate)
                .HasColumnName("updatedDate");
            // .HasColumnType("timestamp(3) without time zone"); // OLD

            builder.Property(e => e.UpdatedUserId)
                .HasColumnName("updatedUserId");

            builder.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.Surname)
                .HasColumnName("surname")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.UserTitle)
                .HasColumnName("userTitle");

            builder.Property(e => e.WorkEndTime)
                .HasColumnName("workEndTime")
                .HasColumnType("date");

            builder.Property(e => e.WorkStartTime)
                .HasColumnName("workStartTime")
                .HasColumnType("date");

            builder.Property(e => e.ForgotPasswordKey)
                .HasColumnName("forgotPasswordKey");

            builder.Property(e => e.ForgotPasswordExpiration)
                .HasColumnName("forgotPasswordExpiration");
            // .HasColumnType("timestamp(3) without time zone"); NEW

            builder.Property(e => e.ShouldChangePassword)
                .HasColumnName("shouldChangePassword")
                .HasDefaultValue(false);

            builder.Property(e => e.VerificationKey)
                .HasColumnName("verificationKey");

            builder.Property(e => e.VerificationKeyExpiration)
                .HasColumnName("verificationKeyExpiration");
            // .HasColumnType("timestamp(3) without time zone"); // OLD
        }

        protected virtual void NotificationConfiguration(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notification");

            builder.Property(e => e.TenantId)
                .HasColumnName("tenantId")
                .HasDefaultValueSql("0");

            builder.Property(e => e.NotificationId)
                .HasColumnName("notificationId");
            // .ValueGeneratedOnAdd(); // ORACLE

            builder.Property(e => e.CreatedDate)
                .HasColumnName("createdDate");

            builder.Property(e => e.CreatedUserId)
                .HasColumnName("createdUserId");

            builder.Property(e => e.DeliveryDate)
                .HasColumnName("deliveryDate");

            builder.Property(e => e.NotificationSettingsId)
                .HasColumnName("notificationSettingsId");
            // builder.HasIndex(e => e.NotificationSettingsId).HasName("IX_Notification_notificationSettingsId"); // ORACLE

            builder.Property(e => e.SendDate)
                .HasColumnName("sendDate");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.UpdatedDate)
                .HasColumnName("updatedDate");

            builder.Property(e => e.UpdatedUserId)
                .HasColumnName("updatedUserId");

            builder.Property(e => e.UserId)
                .HasColumnName("userId");

            builder.HasOne(d => d.NotificationSettings)
                .WithMany(p => p.Notification)
                .HasForeignKey(d => d.NotificationSettingsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notification_settings_fk"); // notification_notificationsettings_fk
        }

        protected virtual void NotificationSettingsConfiguration(EntityTypeBuilder<NotificationSettings> builder)
        {
            builder.ToTable("notificationSettings");

            builder.Property(e => e.NotificationSettingsId)
                .HasColumnName("notificationSettingsId");
            // .ValueGeneratedOnAdd(); // ORACLE

            builder.Property(e => e.AgeRange)
                .HasColumnName("ageRange")
                .HasMaxLength(20);

            builder.Property(e => e.TenantId)
                .HasColumnName("tenantId")
                .HasDefaultValueSql("0");

            builder.Property(e => e.City)
                .HasColumnName("city");

            builder.Property(e => e.CompanyId)
                .HasColumnName("companyId");

            builder.Property(e => e.CreatedDate)
                .HasColumnName("createdDate");

            builder.Property(e => e.CreatedUserId)
                .HasColumnName("createdUserId");

            builder.Property(e => e.Data)
                .HasColumnName("data");

            builder.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(100);

            builder.Property(e => e.Gender)
                .HasColumnName("gender");

            builder.Property(e => e.Message)
                .HasColumnName("message")
                .HasMaxLength(254);

            builder.Property(e => e.NotificationType)
                .HasColumnName("notificationType");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.Subtitle)
                .HasColumnName("subtitle")
                .HasMaxLength(60);

            builder.Property(e => e.Title)
                .HasColumnName("title")
                .HasMaxLength(60);

            builder.Property(e => e.UpdatedDate)
                .HasColumnName("updatedDate");

            builder.Property(e => e.UpdatedUserId)
                .HasColumnName("updatedUserId");

            builder.Property(e => e.UserId)
                .HasColumnName("userId");
        }

        protected virtual void TransactionLogsConfiguration(EntityTypeBuilder<TransactionLogs> builder)
        {
            builder.HasKey(e => e.LogId)
                .HasName("transactionLogs_pkey");

            builder.ToTable("transactionLogs");

            builder.Property(e => e.LogId)
                .HasColumnName("logId");

            builder.Property(e => e.TenantId)
                .HasColumnName("tenantId")
                .HasDefaultValueSql("0");

            builder.Property(e => e.CurrentClaims)
                .HasColumnName("currentClaims")
                .HasJsonConversion();

            builder.Property(e => e.LogDateBegin)
                .HasColumnName("logDateBegin")
                // .ValueGeneratedNever() // ORACLE
                .HasDefaultValueSql(DATE_DEFAULT_VALUE);

            builder.Property(e => e.LogDateEnd)
                .HasColumnName("logDateEnd");

            builder.Property(e => e.LogType)
                .HasColumnName("logType");

            builder.Property(e => e.Request)
                .HasColumnName("request")
                .HasJsonConversion();

            builder.Property(e => e.Response)
                .HasColumnName("response")
                .HasJsonConversion();

            builder.Property(e => e.ServiceUrl)
                .HasColumnName("serviceUrl")
                .HasMaxLength(254)
                .IsRequired();

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.UserId)
                .HasColumnName("userId");

            builder.Property(e => e.StatusCode)
                .HasColumnName("statusCode");
        }

        protected virtual void TenantConfiguration(EntityTypeBuilder<Tenant> builder)
        {
            builder.HasKey(e => e.TenantId);

            builder.ToTable("tenants");

            builder.Property(e => e.TenantId)
                .HasColumnName("tenantId");

            builder.Property(e => e.ParentTenantId)
                .HasColumnName("parentTenantId");

            builder.Property(e => e.TenantName)
                .HasColumnName("tenantName");

            builder.Property(e => e.TenantType)
                .HasColumnName("tenantType")
                .IsRequired();

            builder.Property(e => e.Address)
                .HasColumnName("address");

            builder.Property(e => e.Email)
                .HasColumnName("email");

            builder.Property(e => e.PhoneNumber)
                .HasColumnName("phoneNumber");

            builder.Property(e => e.TaxOffice)
                .HasColumnName("taxOffice")
                .HasMaxLength(50);

            builder.Property(e => e.TaxNumber)
                .HasColumnName("taxNumber")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(e => e.CountryId)
                .HasColumnName("countryId");

            builder.Property(e => e.CityId)
                .HasColumnName("cityId");

            builder.Property(e => e.Note)
                .HasColumnName("note")
                .HasMaxLength(500);

            builder.Property(e => e.Website)
                .HasColumnName("website");

            builder.Property(e => e.CreatedUserId)
                .HasColumnName("createdUserId");

            builder.Property(e => e.CreatedDate)
                .HasColumnName("createdDate");

            builder.Property(e => e.UpdatedDate)
                .HasColumnName("updatedDate");

            builder.Property(e => e.UpdatedUserId)
                .HasColumnName("updatedUserId");

            builder.Property(e => e.Status)
                .HasColumnName("status");

            builder.Property(e => e.IsDefault)
                .HasColumnName("isDefault")
                .HasDefaultValue(false);
        }

        protected virtual void SampleEmployeeConfiguration(EntityTypeBuilder<SampleEmployee> builder)
        {
            builder.HasKey(e => e.EmployeeId);

            builder.ToTable("sampleEmployee");

            builder.HasIndex(e => e.EmployeeId);

            builder.Property(e => e.EmployeeId)
                .HasColumnName("employeeId");

            builder.Property(e => e.EmployeeName)
                .HasColumnName("employeeName")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(e => e.EmployeeSurname)
                .HasColumnName("employeeSurname")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.CompanyId)
                .HasColumnName("companyId");

            builder.Property(e => e.EmployeeTitle)
                .HasColumnName("employeeTitle");

            builder.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(e => e.Password)
                .HasColumnName("password")
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(e => e.Gender)
                .HasColumnName("gender");

            builder.Property(e => e.PhoneNumber)
                .HasColumnName("phoneNumber");

            builder.Property(e => e.IbanNumber)
                .HasColumnName("ibanNumber");

            builder.Property(e => e.TaxNumber)
                .HasColumnName("taxNumber")
                .IsRequired();

            builder.Property(e => e.CityId)
                .HasColumnName("cityId");

            builder.Property(e => e.CountyId)
                .HasColumnName("countyId");

            builder.Property(e => e.WorkStartDate)
                .HasColumnName("workStartDate");

            builder.Property(e => e.Salary)
                .HasColumnName("salary");

            builder.Property(e => e.Note)
                .HasColumnName("note");

            builder.Property(e => e.Picture)
                .HasColumnName("picture")
                .HasJsonConversion();

            builder.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValue(1);
        }

        protected virtual void SampleEmployeeTaskConfiguration(EntityTypeBuilder<SampleEmployeeTask> builder)
        {
            builder.HasKey(e => e.EmployeeTaskId);

            builder.ToTable("sampleEmployeeTask");

            builder.Property(e => e.EmployeeTaskId)
                .HasColumnName("employeeTaskId");

            builder.Property(e => e.EmployeeId)
                .HasColumnName("employeeId");

            builder.Property(e => e.EmployeeTaskName)
                .HasColumnName("employeeTaskName");

            builder.Property(e => e.EmployeeTaskDescription)
                .HasColumnName("employeeTaskDescription");

            builder.Property(e => e.TaskTags)
                .HasColumnName("taskTags")
                .HasJsonConversion();

            builder.Property(e => e.DueDate)
                .HasColumnName("dueDate");

            builder.Property(e => e.Status)
                .HasColumnName("status")
                .HasDefaultValue(1);
        }
    }
}