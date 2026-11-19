import http from 'k6/http';
import { check } from 'k6';

const baseUrl = __ENV.BASE_URL || 'http://localhost:8080';
const path = __ENV.TARGET_PATH || '/api/Disk/disks';
const requiresToken = /^\/api(?:\/|\?|$)/i.test(path);
const accessToken = __ENV.MYDISK_ACCESS_TOKEN;

if (requiresToken && !accessToken) {
  throw new Error('Set MYDISK_ACCESS_TOKEN to a Keycloak access token for the mydisk-api audience.');
}

const requestOptions = {
  redirects: 0,
  headers: requiresToken ? { Authorization: `Bearer ${accessToken}` } : {},
};

const lightRate = Number(__ENV.LIGHT_RATE || 5);
const mediumRate = Number(__ENV.MEDIUM_RATE || 150);
const heavyRate = Number(__ENV.HEAVY_RATE || 600);

function phase(rate, startTime, duration) {
  return {
    executor: 'constant-arrival-rate',
    rate,
    timeUnit: '1s',
    duration,
    startTime,
    preAllocatedVUs: Math.max(5, Math.ceil(rate / 10)),
    maxVUs: Math.max(20, rate),
  };
}

export const options = {
  scenarios: {
    light: phase(lightRate, '0s', '30s'),
    medium: phase(mediumRate, '30s', '60s'),
    heavy: phase(heavyRate, '90s', '60s'),
    cooldown: phase(lightRate, '150s', '60s'),
  },
};

export default function () {
  const response = http.get(`${baseUrl}${path}`, requestOptions);

  check(response, {
    'status is 200': (r) => r.status === 200,
  });
}
