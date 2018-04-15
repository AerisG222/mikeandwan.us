// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,
  authUrl: 'https://authdev.mikeandwan.us:5001',
  apiUrl: 'https://apidev.mikeandwan.us:5011',
  wwwUrl: 'https://wwwdev.mikeandwan.us:5021'
};
