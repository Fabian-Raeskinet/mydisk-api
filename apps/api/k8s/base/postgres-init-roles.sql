\getenv db POSTGRES_DB
\getenv migrator DB_MIGRATOR_USER
\getenv app DB_APP_USER
\set migrator_password `cat /run/secrets/postgres/POSTGRES_MIGRATOR_PASSWORD`
\set app_password `cat /run/secrets/postgres/POSTGRES_APP_PASSWORD`

REVOKE ALL ON DATABASE :"db", postgres FROM PUBLIC;

CREATE ROLE :"migrator" LOGIN PASSWORD :'migrator_password';
CREATE ROLE :"app" LOGIN PASSWORD :'app_password';

GRANT CONNECT ON DATABASE :"db" TO :"migrator", :"app";
GRANT USAGE, CREATE ON SCHEMA public TO :"migrator";
GRANT USAGE ON SCHEMA public TO :"app";

ALTER DEFAULT PRIVILEGES FOR ROLE :"migrator" IN SCHEMA public
  GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO :"app";
ALTER DEFAULT PRIVILEGES FOR ROLE :"migrator" IN SCHEMA public
  GRANT USAGE ON SEQUENCES TO :"app";
