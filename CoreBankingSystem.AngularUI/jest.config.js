/* eslint-disable */
module.exports = {
  preset: 'jest-preset-angular',
  roots: ['<rootDir>/src'],
  testMatch: ['**/__tests__/**/*.spec.ts', '**/*.spec.ts'],
  setupFilesAfterEnv: ['<rootDir>/src/setup-jest.ts'],
  transform: {
    '^.+\\.(ts|mjs|html)$': [
      'jest-preset-angular',
      {
        tsconfig: '<rootDir>/tsconfig.jest.json'
      }
    ]
  },
  moduleFileExtensions: ['ts', 'html', 'js', 'json'],
  testEnvironment: 'jsdom',
  globals: {},
  coverageDirectory: '<rootDir>/coverage-jest',
  collectCoverageFrom: [
    'src/app/**/*.{ts,html}',
    '!src/main.ts',
    '!src/polyfills.ts',
    '!src/environments/**/*.ts',
    '!src/**/*.module.ts'
  ],
  moduleNameMapper: {
    '\\.(css|scss)$': 'identity-obj-proxy'
  },
  transformIgnorePatterns: [
    'node_modules/(?!.*)'
  ]
};
