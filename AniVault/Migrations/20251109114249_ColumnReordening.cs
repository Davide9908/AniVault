using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AniVault.Migrations
{
    /// <inheritdoc />
    public partial class ColumnReordening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "telegram_message_id",
                table: "telegram_media_document",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<long>(
                name: "size",
                table: "telegram_media_document",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Relational:ColumnOrder", 12);

            migrationBuilder.AlterColumn<short>(
                name: "retries",
                table: "telegram_media_document",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint")
                .Annotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<string>(
                name: "mime_type",
                table: "telegram_media_document",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30)
                .Annotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<DateTime>(
                name: "last_update_date_time",
                table: "telegram_media_document",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<string>(
                name: "filename_to_update",
                table: "telegram_media_document",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 10);

            migrationBuilder.AlterColumn<string>(
                name: "filename_from_telegram",
                table: "telegram_media_document",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100)
                .Annotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<string>(
                name: "filename",
                table: "telegram_media_document",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80)
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<byte[]>(
                name: "file_reference",
                table: "telegram_media_document",
                type: "bytea",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea")
                .Annotation("Relational:ColumnOrder", 15);

            migrationBuilder.AlterColumn<long>(
                name: "file_id",
                table: "telegram_media_document",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "download_status",
                table: "telegram_media_document",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<long>(
                name: "data_transmitted",
                table: "telegram_media_document",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Relational:ColumnOrder", 13);

            migrationBuilder.AlterColumn<DateTime>(
                name: "creation_date_time",
                table: "telegram_media_document",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone")
                .Annotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<int>(
                name: "anime_configuration_id",
                table: "telegram_media_document",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<long>(
                name: "access_hash",
                table: "telegram_media_document",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Relational:ColumnOrder", 14);

            migrationBuilder.AlterColumn<int>(
                name: "telegram_media_document_id",
                table: "telegram_media_document",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .Annotation("Relational:ColumnOrder", 0)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
            
            var sqlScript = @"

CREATE TABLE public.telegram_media_document_new
(
    telegram_media_document_id integer GENERATED BY DEFAULT AS IDENTITY
        CONSTRAINT pk_telegram_media_document_temp PRIMARY KEY,
    file_id                    bigint                   NOT NULL,
    filename                   varchar(80)              NOT NULL,
    download_status            integer                  NOT NULL,
    creation_date_time         timestamp with time zone NOT NULL,
    last_update_date_time      timestamp with time zone NOT NULL,
    retries                    smallint default 0       not null,
    anime_configuration_id     integer                  NOT NULL
        CONSTRAINT fk_telegram_media_document_anime_configuration_temp
            REFERENCES public.anime_configuration
            ON DELETE CASCADE,
    telegram_message_id        integer                  NOT NULL
        CONSTRAINT fk_telegram_media_document_telegram_message_temp
            REFERENCES public.telegram_message
            ON DELETE CASCADE,
    filename_from_telegram     varchar(100)             NOT NULL,
    filename_to_update         varchar(80),
    mime_type                  varchar(30)              NOT NULL,
    size                       bigint                   NOT NULL,
    data_transmitted           bigint                   NOT NULL,
    access_hash                bigint                   NOT NULL,
    file_reference             bytea                    NOT NULL
);

ALTER TABLE public.telegram_media_document_new
    OWNER TO postgres;

CREATE INDEX ix_telegram_media_document_anime_configuration_id_temp
    ON public.telegram_media_document_new (anime_configuration_id);

CREATE UNIQUE INDEX ix_telegram_media_document_file_id_telegram_message_id_temp
    ON public.telegram_media_document_new (file_id, telegram_message_id);

CREATE UNIQUE INDEX ix_telegram_media_document_telegram_message_id_temp
    ON public.telegram_media_document_new (telegram_message_id);

INSERT INTO public.telegram_media_document_new (
    telegram_media_document_id, file_id, filename, download_status,
    creation_date_time, last_update_date_time, retries, anime_configuration_id,
    telegram_message_id, filename_from_telegram, filename_to_update,
    mime_type, size, data_transmitted, access_hash, file_reference
)
OVERRIDING SYSTEM VALUE
SELECT
    telegram_media_document_id, file_id, filename, download_status,
    creation_date_time, last_update_date_time, retries, anime_configuration_id,
    telegram_message_id, filename_from_telegram, filename_to_update,
    mime_type, size, data_transmitted, access_hash, file_reference
FROM public.telegram_media_document;


SELECT setval(
    pg_get_serial_sequence('public.telegram_media_document_new', 'telegram_media_document_id'),
    COALESCE(MAX(telegram_media_document_id), 1),
    (MAX(telegram_media_document_id) IS NOT NULL)
) FROM public.telegram_media_document_new;

DROP TABLE public.telegram_media_document CASCADE;

ALTER TABLE public.telegram_media_document_new
    RENAME TO telegram_media_document;


ALTER INDEX pk_telegram_media_document_temp
    RENAME TO pk_telegram_media_document;

ALTER TABLE public.telegram_media_document
    RENAME CONSTRAINT fk_telegram_media_document_anime_configuration_temp
    TO fk_telegram_media_document_anime_configuration_anime_configura;

ALTER TABLE public.telegram_media_document
    RENAME CONSTRAINT fk_telegram_media_document_telegram_message_temp
    TO fk_telegram_media_document_telegram_message_telegram_message_id;

ALTER INDEX ix_telegram_media_document_anime_configuration_id_temp
    RENAME TO ix_telegram_media_document_anime_configuration_id;

ALTER INDEX ix_telegram_media_document_file_id_telegram_message_id_temp
    RENAME TO ix_telegram_media_document_file_id_telegram_message_id;

ALTER INDEX ix_telegram_media_document_telegram_message_id_temp
    RENAME TO ix_telegram_media_document_telegram_message_id;
";
            migrationBuilder.Sql(sqlScript);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "telegram_message_id",
                table: "telegram_media_document",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<long>(
                name: "size",
                table: "telegram_media_document",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Relational:ColumnOrder", 12);

            migrationBuilder.AlterColumn<short>(
                name: "retries",
                table: "telegram_media_document",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint")
                .OldAnnotation("Relational:ColumnOrder", 6);

            migrationBuilder.AlterColumn<string>(
                name: "mime_type",
                table: "telegram_media_document",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30)
                .OldAnnotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<DateTime>(
                name: "last_update_date_time",
                table: "telegram_media_document",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone")
                .OldAnnotation("Relational:ColumnOrder", 5);

            migrationBuilder.AlterColumn<string>(
                name: "filename_to_update",
                table: "telegram_media_document",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80,
                oldNullable: true)
                .OldAnnotation("Relational:ColumnOrder", 10);

            migrationBuilder.AlterColumn<string>(
                name: "filename_from_telegram",
                table: "telegram_media_document",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100)
                .OldAnnotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<string>(
                name: "filename",
                table: "telegram_media_document",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80)
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder.AlterColumn<byte[]>(
                name: "file_reference",
                table: "telegram_media_document",
                type: "bytea",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea")
                .OldAnnotation("Relational:ColumnOrder", 15);

            migrationBuilder.AlterColumn<long>(
                name: "file_id",
                table: "telegram_media_document",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "download_status",
                table: "telegram_media_document",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Relational:ColumnOrder", 3);

            migrationBuilder.AlterColumn<long>(
                name: "data_transmitted",
                table: "telegram_media_document",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Relational:ColumnOrder", 13);

            migrationBuilder.AlterColumn<DateTime>(
                name: "creation_date_time",
                table: "telegram_media_document",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone")
                .OldAnnotation("Relational:ColumnOrder", 4);

            migrationBuilder.AlterColumn<int>(
                name: "anime_configuration_id",
                table: "telegram_media_document",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<long>(
                name: "access_hash",
                table: "telegram_media_document",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Relational:ColumnOrder", 14);

            migrationBuilder.AlterColumn<int>(
                name: "telegram_media_document_id",
                table: "telegram_media_document",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Relational:ColumnOrder", 0);
            
            var sqlScript = @"
CREATE TABLE public.telegram_media_document_revert
(
    telegram_media_document_id integer GENERATED BY DEFAULT AS IDENTITY
        CONSTRAINT pk_telegram_media_document_temp_revert PRIMARY KEY,
    file_id                    bigint                   NOT NULL,
    access_hash                bigint                   NOT NULL,
    file_reference             bytea                    NOT NULL,
    telegram_message_id        integer                  NOT NULL
        CONSTRAINT fk_telegram_media_document_telegram_message_temp_revert
            REFERENCES public.telegram_message
            ON DELETE CASCADE,
    anime_configuration_id     integer                  NOT NULL
        CONSTRAINT fk_telegram_media_document_anime_configuration_temp_revert
            REFERENCES public.anime_configuration
            ON DELETE CASCADE,
    filename                   varchar(80)              NOT NULL,
    filename_from_telegram     varchar(100)             NOT NULL,
    filename_to_update         varchar(80),
    mime_type                  varchar(30)              NOT NULL,
    size                       bigint                   NOT NULL,
    data_transmitted           bigint                   NOT NULL,
    creation_date_time         timestamp with time zone NOT NULL,
    last_update_date_time      timestamp with time zone NOT NULL,
    download_status            integer                  NOT NULL,
    retries                    smallint default 0       not null
);

ALTER TABLE public.telegram_media_document_revert
    OWNER TO postgres;

CREATE INDEX ix_telegram_media_document_anime_config_id_temp_revert
    ON public.telegram_media_document_revert (anime_configuration_id);

CREATE UNIQUE INDEX ix_telegram_media_document_file_id_msg_id_temp_revert
    ON public.telegram_media_document_revert (file_id, telegram_message_id);

CREATE UNIQUE INDEX ix_telegram_media_document_telegram_message_id_temp_revert
    ON public.telegram_media_document_revert (telegram_message_id);

INSERT INTO public.telegram_media_document_revert (
    telegram_media_document_id, file_id, access_hash, file_reference,
    telegram_message_id, anime_configuration_id, filename,
    filename_from_telegram, filename_to_update, mime_type,
    size, data_transmitted, creation_date_time,
    last_update_date_time, download_status, retries
)
OVERRIDING SYSTEM VALUE
SELECT
    telegram_media_document_id, file_id, access_hash, file_reference,
    telegram_message_id, anime_configuration_id, filename,
    filename_from_telegram, filename_to_update, mime_type,
    size, data_transmitted, creation_date_time,
    last_update_date_time, download_status, retries
FROM public.telegram_media_document; 

SELECT setval(
    pg_get_serial_sequence('public.telegram_media_document_revert', 'telegram_media_document_id'),
    COALESCE(MAX(telegram_media_document_id), 1),
    (MAX(telegram_media_document_id) IS NOT NULL)
) FROM public.telegram_media_document_revert;

DROP TABLE public.telegram_media_document CASCADE;

ALTER TABLE public.telegram_media_document_revert
    RENAME TO telegram_media_document;

ALTER INDEX pk_telegram_media_document_temp_revert
    RENAME TO pk_telegram_media_document;

ALTER TABLE public.telegram_media_document
    RENAME CONSTRAINT fk_telegram_media_document_telegram_message_temp_revert
    TO fk_telegram_media_document_telegram_message_telegram_message_id;

ALTER TABLE public.telegram_media_document
    RENAME CONSTRAINT fk_telegram_media_document_anime_configuration_temp_revert
    TO fk_telegram_media_document_anime_configuration_anime_configura;

ALTER INDEX ix_telegram_media_document_anime_config_id_temp_revert
    RENAME TO ix_telegram_media_document_anime_configuration_id;

ALTER INDEX ix_telegram_media_document_file_id_msg_id_temp_revert
    RENAME TO ix_telegram_media_document_file_id_telegram_message_id;

ALTER INDEX ix_telegram_media_document_telegram_message_id_temp_revert
    RENAME TO ix_telegram_media_document_telegram_message_id;
";
            migrationBuilder.Sql(sqlScript);
        }
    }
}
