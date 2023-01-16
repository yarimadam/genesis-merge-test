using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CoreData.Migrations
{
    public partial class migration_genesisContext_PostgreSQL_202301162136 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "authResources",
                columns: table => new
                {
                    resourceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    parentResourceCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    resourceCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    resourceName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    resourceType = table.Column<int>(type: "integer", nullable: false),
                    orderIndex = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    seoTitle = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: true, comment: "While technically not a meta tag, this tag is often used together with the \"description\". The contents of this tag are generally shown as the title in search results (and of course in the user's browser)."),
                    seoDescription = table.Column<string>(type: "character varying(175)", maxLength: 175, nullable: true, comment: "This tag provides a short description of the page."),
                    seoKeywords = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: true),
                    tableName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    tenantId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0"),
                    createdUserId = table.Column<int>(type: "integer", nullable: true),
                    createdDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updatedUserId = table.Column<int>(type: "integer", nullable: true),
                    updatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authResources", x => x.resourceId);
                });

            migrationBuilder.CreateTable(
                name: "authTemplate",
                columns: table => new
                {
                    authTemplateId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    templateName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    templateDefaultPage = table.Column<string>(type: "text", nullable: true),
                    templateType = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    isDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    tenantId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authTemplate", x => x.authTemplateId);
                });

            migrationBuilder.CreateTable(
                name: "communicationDefinitions",
                columns: table => new
                {
                    commDefinitionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    commDefinitionName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    commDefinitionType = table.Column<short>(type: "smallint", nullable: false),
                    emailSenderName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    emailUsername = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    emailPassword = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    emailSmtpServer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    emailSenderAccount = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    emailSecurityType = table.Column<short>(type: "smallint", nullable: true),
                    emailPort = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    smsCompanyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    smsProviderCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    smsCustomerCode = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: true),
                    smsSenderNumber = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: true),
                    smsEndpointUrl = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    smsAuthData = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    smsFormData = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    smsExpectedStatusCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    smsExpectedResponse = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    smsUsername = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: true),
                    smsPassword = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    companyId = table.Column<int>(type: "integer", nullable: true),
                    timezone = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    tenantId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0"),
                    createdUserId = table.Column<int>(type: "integer", nullable: true),
                    createdDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updatedUserId = table.Column<int>(type: "integer", nullable: true),
                    updatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_communicationDefinitions", x => x.commDefinitionId);
                });

            migrationBuilder.CreateTable(
                name: "communicationTemplates",
                columns: table => new
                {
                    commTemplateId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    commTemplateName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    commDefinitionId = table.Column<int>(type: "integer", nullable: false),
                    emailRecipients = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    emailCCs = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    emailBCCs = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    emailSubject = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    emailBody = table.Column<string>(type: "character varying", nullable: true),
                    emailIsBodyHtml = table.Column<bool>(type: "boolean", nullable: true),
                    emailPriority = table.Column<short>(type: "smallint", nullable: true),
                    emailSenderName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    smsRecipients = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    smsBody = table.Column<string>(type: "character varying", nullable: true),
                    companyId = table.Column<int>(type: "integer", nullable: true),
                    timezone = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    serviceUrls = table.Column<string>(type: "text", nullable: true),
                    requestType = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    responseType = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    requestConditions = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    tenantId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0"),
                    createdUserId = table.Column<int>(type: "integer", nullable: true),
                    createdDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updatedUserId = table.Column<int>(type: "integer", nullable: true),
                    updatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_communicationTemplates", x => x.commTemplateId);
                });

            migrationBuilder.CreateTable(
                name: "coreCompany",
                columns: table => new
                {
                    companyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    companyName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    companyLegalTitle = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    sectorId = table.Column<int>(type: "integer", nullable: true),
                    numberOfStaff = table.Column<int>(type: "integer", nullable: true),
                    taxOffice = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    taxNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    billingAddress = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    contactPerson = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: true),
                    contactPersonTitle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    contactPersonTelephone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    contactPersonEmail = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: true),
                    countryId = table.Column<int>(type: "integer", nullable: true),
                    cityId = table.Column<int>(type: "integer", nullable: true),
                    townId = table.Column<int>(type: "integer", nullable: true),
                    address = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    telephone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    website = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    tenantId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0"),
                    createdUserId = table.Column<int>(type: "integer", nullable: true),
                    createdDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updatedUserId = table.Column<int>(type: "integer", nullable: true),
                    updatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coreCompany", x => x.companyId);
                });

            migrationBuilder.CreateTable(
                name: "coreParameters",
                columns: table => new
                {
                    parameterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    keyCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    parentValue = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    orderIndex = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    translations = table.Column<string>(type: "json", nullable: true),
                    tenantId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parameters", x => x.parameterId);
                });

            migrationBuilder.CreateTable(
                name: "coreUsers",
                columns: table => new
                {
                    userId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    departmentId = table.Column<int>(type: "integer", nullable: true),
                    relationType = table.Column<int>(type: "integer", nullable: true),
                    relatedUserId = table.Column<int>(type: "integer", nullable: true),
                    ibanNumber = table.Column<string>(type: "character varying(33)", maxLength: 33, nullable: true),
                    countyId = table.Column<int>(type: "integer", nullable: true),
                    cityId = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    surname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    companyId = table.Column<int>(type: "integer", nullable: true),
                    email = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    gender = table.Column<int>(type: "integer", nullable: true),
                    birthDate = table.Column<DateTime>(type: "date", nullable: true),
                    registrationNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    identificationNo = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    password = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    tempPassword = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    workStartTime = table.Column<DateTime>(type: "date", nullable: true),
                    workEndTime = table.Column<DateTime>(type: "date", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    maritalStatus = table.Column<int>(type: "integer", nullable: true),
                    phoneNumber = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    userTitle = table.Column<int>(type: "integer", nullable: true),
                    generalUser = table.Column<int>(type: "integer", nullable: true),
                    marriageDate = table.Column<DateTime>(type: "date", nullable: true),
                    isAuthorized = table.Column<short>(type: "smallint", nullable: true),
                    roleId = table.Column<int>(type: "integer", nullable: true),
                    identityType = table.Column<short>(type: "smallint", nullable: true),
                    forgotPasswordKey = table.Column<string>(type: "text", nullable: true),
                    forgotPasswordExpiration = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    shouldChangePassword = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    verificationKey = table.Column<string>(type: "text", nullable: true),
                    verificationKeyExpiration = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    tenantId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0"),
                    createdUserId = table.Column<int>(type: "integer", nullable: true),
                    createdDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updatedUserId = table.Column<int>(type: "integer", nullable: true),
                    updatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "notificationSettings",
                columns: table => new
                {
                    notificationSettingsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    notificationType = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    subtitle = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    message = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    data = table.Column<string>(type: "json", nullable: true),
                    userId = table.Column<int>(type: "integer", nullable: true),
                    companyId = table.Column<int>(type: "integer", nullable: true),
                    gender = table.Column<int>(type: "integer", nullable: true),
                    city = table.Column<int>(type: "integer", nullable: true),
                    ageRange = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenantId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0"),
                    createdUserId = table.Column<int>(type: "integer", nullable: true),
                    createdDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updatedUserId = table.Column<int>(type: "integer", nullable: true),
                    updatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notificationSettings", x => x.notificationSettingsId);
                });

            migrationBuilder.CreateTable(
                name: "sampleEmployee",
                columns: table => new
                {
                    employeeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    employeeName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    employeeSurname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    companyId = table.Column<int>(type: "integer", nullable: false),
                    employeeTitle = table.Column<int>(type: "integer", nullable: true),
                    email = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    password = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    gender = table.Column<short>(type: "smallint", nullable: true),
                    phoneNumber = table.Column<string>(type: "text", nullable: true),
                    ibanNumber = table.Column<string>(type: "text", nullable: true),
                    taxNumber = table.Column<string>(type: "text", nullable: false),
                    cityId = table.Column<int>(type: "integer", nullable: true),
                    countyId = table.Column<int>(type: "integer", nullable: true),
                    workStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    salary = table.Column<decimal>(type: "numeric", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    picture = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sampleEmployee", x => x.employeeId);
                });

            migrationBuilder.CreateTable(
                name: "sampleEmployeeTask",
                columns: table => new
                {
                    employeeTaskId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    employeeId = table.Column<int>(type: "integer", nullable: false),
                    employeeTaskName = table.Column<string>(type: "text", nullable: true),
                    employeeTaskDescription = table.Column<string>(type: "text", nullable: true),
                    taskTags = table.Column<string>(type: "text", nullable: true),
                    dueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sampleEmployeeTask", x => x.employeeTaskId);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    tenantId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    parentTenantId = table.Column<int>(type: "integer", nullable: true),
                    tenantName = table.Column<string>(type: "text", nullable: true),
                    tenantType = table.Column<int>(type: "integer", nullable: false),
                    address = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    phoneNumber = table.Column<string>(type: "text", nullable: true),
                    taxOffice = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    taxNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    countryId = table.Column<int>(type: "integer", nullable: true),
                    cityId = table.Column<int>(type: "integer", nullable: true),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    website = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    isDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    createdUserId = table.Column<int>(type: "integer", nullable: true),
                    createdDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updatedUserId = table.Column<int>(type: "integer", nullable: true),
                    updatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.tenantId);
                    table.ForeignKey(
                        name: "FK_tenants_tenants_parentTenantId",
                        column: x => x.parentTenantId,
                        principalTable: "tenants",
                        principalColumn: "tenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transactionLogs",
                columns: table => new
                {
                    logId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    userId = table.Column<int>(type: "integer", nullable: false),
                    serviceUrl = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    request = table.Column<string>(type: "json", nullable: true),
                    response = table.Column<string>(type: "json", nullable: true),
                    currentClaims = table.Column<string>(type: "json", nullable: true),
                    logType = table.Column<int>(type: "integer", nullable: false),
                    logDateBegin = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    logDateEnd = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    statusCode = table.Column<int>(type: "integer", nullable: true),
                    tenantId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("transactionLogs_pkey", x => x.logId);
                });

            migrationBuilder.CreateTable(
                name: "authActions",
                columns: table => new
                {
                    actionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    resourceId = table.Column<int>(type: "integer", nullable: false),
                    actionType = table.Column<int>(type: "integer", nullable: false),
                    orderIndex = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    tenantId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0"),
                    createdUserId = table.Column<int>(type: "integer", nullable: true),
                    createdDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updatedUserId = table.Column<int>(type: "integer", nullable: true),
                    updatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authActions", x => x.actionId);
                    table.ForeignKey(
                        name: "authActions_resourceId_fkey",
                        column: x => x.resourceId,
                        principalTable: "authResources",
                        principalColumn: "resourceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "authTemplateDetail",
                columns: table => new
                {
                    authTemplateDetailId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    authTemplateId = table.Column<int>(type: "integer", nullable: false),
                    resourceId = table.Column<int>(type: "integer", nullable: false),
                    actionId = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenantId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authTemplateDetail", x => x.authTemplateDetailId);
                    table.ForeignKey(
                        name: "authTemplateDetail.authTemplateId.fkey",
                        column: x => x.authTemplateId,
                        principalTable: "authTemplate",
                        principalColumn: "authTemplateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "coreDepartment",
                columns: table => new
                {
                    departmentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    companyId = table.Column<int>(type: "integer", nullable: true),
                    departmentName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    parentDepartmentId = table.Column<int>(type: "integer", nullable: true),
                    depHeadUserId = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    tenantId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0"),
                    createdUserId = table.Column<int>(type: "integer", nullable: true),
                    createdDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updatedUserId = table.Column<int>(type: "integer", nullable: true),
                    updatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coreDepartment", x => x.departmentId);
                    table.ForeignKey(
                        name: "coredepartment_corecompany_fk",
                        column: x => x.companyId,
                        principalTable: "coreCompany",
                        principalColumn: "companyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "coredepartment_coreuser_depheaduserid_fk",
                        column: x => x.depHeadUserId,
                        principalTable: "coreUsers",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    notificationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    notificationSettingsId = table.Column<int>(type: "integer", nullable: false),
                    userId = table.Column<int>(type: "integer", nullable: true),
                    sendDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deliveryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenantId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0"),
                    createdUserId = table.Column<int>(type: "integer", nullable: true),
                    createdDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updatedUserId = table.Column<int>(type: "integer", nullable: true),
                    updatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.notificationId);
                    table.ForeignKey(
                        name: "notification_settings_fk",
                        column: x => x.notificationSettingsId,
                        principalTable: "notificationSettings",
                        principalColumn: "notificationSettingsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "authUserRights",
                columns: table => new
                {
                    rightId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    actionId = table.Column<int>(type: "integer", nullable: false),
                    userId = table.Column<int>(type: "integer", nullable: false),
                    recordType = table.Column<int>(type: "integer", nullable: false),
                    groupActionId = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenantId = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "0"),
                    createdUserId = table.Column<int>(type: "integer", nullable: true),
                    createdDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "now()"),
                    updatedUserId = table.Column<int>(type: "integer", nullable: true),
                    updatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authUserRights", x => x.rightId);
                    table.ForeignKey(
                        name: "authUserRights_actionId_fkey",
                        column: x => x.actionId,
                        principalTable: "authActions",
                        principalColumn: "actionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "authUserRights_userId_fkey",
                        column: x => x.userId,
                        principalTable: "coreUsers",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "authActions_resourceId_and_actionType_must_be_unique",
                table: "authActions",
                columns: new[] { "resourceId", "actionType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthActions_resourceId",
                table: "authActions",
                column: "resourceId");

            migrationBuilder.CreateIndex(
                name: "ResourceCode_must_be_unique",
                table: "authResources",
                column: "resourceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthTemplateDetail_authTemplateDetail",
                table: "authTemplateDetail",
                column: "authTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUserRights_ActionId",
                table: "authUserRights",
                column: "actionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUserRights_UserId",
                table: "authUserRights",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "CompanyName_must_be_unique",
                table: "coreCompany",
                column: "companyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_coreDepartment_companyId",
                table: "coreDepartment",
                column: "companyId");

            migrationBuilder.CreateIndex(
                name: "IX_coreDepartment_depHeadUserId",
                table: "coreDepartment",
                column: "depHeadUserId");

            migrationBuilder.CreateIndex(
                name: "KEYCODE_AND_VALUE_PAIR_MUST_BE_UNIQUE",
                table: "coreParameters",
                columns: new[] { "tenantId", "keyCode", "parentValue", "value", "status" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EMAIL_MUST_BE_UNIQUE",
                table: "coreUsers",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notification_notificationSettingsId",
                table: "notification",
                column: "notificationSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_sampleEmployee_employeeId",
                table: "sampleEmployee",
                column: "employeeId");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_parentTenantId",
                table: "tenants",
                column: "parentTenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "authTemplateDetail");

            migrationBuilder.DropTable(
                name: "authUserRights");

            migrationBuilder.DropTable(
                name: "communicationDefinitions");

            migrationBuilder.DropTable(
                name: "communicationTemplates");

            migrationBuilder.DropTable(
                name: "coreDepartment");

            migrationBuilder.DropTable(
                name: "coreParameters");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "sampleEmployee");

            migrationBuilder.DropTable(
                name: "sampleEmployeeTask");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropTable(
                name: "transactionLogs");

            migrationBuilder.DropTable(
                name: "authTemplate");

            migrationBuilder.DropTable(
                name: "authActions");

            migrationBuilder.DropTable(
                name: "coreCompany");

            migrationBuilder.DropTable(
                name: "coreUsers");

            migrationBuilder.DropTable(
                name: "notificationSettings");

            migrationBuilder.DropTable(
                name: "authResources");
        }
    }
}
