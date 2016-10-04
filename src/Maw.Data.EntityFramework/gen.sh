#!/bin/bash

echo 'enter pgsql username:';
read PG_USER;

echo 'enter pgsql password:';
read -s PG_PWD;

CONN="Server=localhost;Port=5432;Database=maw_website;User Id=${PG_USER};Password=${PG_PWD}"
PROV='Npgsql.EntityFrameworkCore.PostgreSQL'

#-- blog
dotnet ef dbcontext scaffold "$CONN" "$PROV" -f --data-annotations --context BlogContext --output-dir Blogs --schema blog

#-- identity
dotnet ef dbcontext scaffold "$CONN" "$PROV" -f --data-annotations --context IdentityContext --output-dir Identity --schema maw

#-- photo
dotnet ef dbcontext scaffold "$CONN" "$PROV" -f --data-annotations --context PhotoContext --output-dir Photos --schema photo --table maw.user

#-- video
dotnet ef dbcontext scaffold "$CONN" "$PROV" -f --data-annotations --context VideoContext --output-dir Videos --schema video
