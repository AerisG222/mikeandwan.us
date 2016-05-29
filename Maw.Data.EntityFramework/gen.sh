#!/bin/bash

echo 'enter pgsql username:';
read PG_USER;

echo 'enter pgsql password:';
read -s PG_PWD;

CONN="Server=localhost;Port=5432;Database=maw_website;User Id=${PG_USER};Password=${PG_PWD}"
PROV='Npgsql.EntityFrameworkCore.PostgreSQL'

#-- blog
dotnet ef dbcontext scaffold "$CONN" "$PROV" --dataAnnotations --context BlogContext --outputDir Blogs --schema blog

#-- identity
dotnet ef dbcontext scaffold "$CONN" "$PROV" --dataAnnotations --context IdentityContext --outputDir Identity --schema maw

#-- photo
dotnet ef dbcontext scaffold "$CONN" "$PROV" --dataAnnotations --context PhotoContext --outputDir Photos --schema photo --table maw.user

#-- video
dotnet ef dbcontext scaffold "$CONN" "$PROV" --dataAnnotations --context VideoContext --outputDir Videos --schema video
