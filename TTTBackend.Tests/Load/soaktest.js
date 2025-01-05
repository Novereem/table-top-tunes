import http from 'k6/http';
import { sleep, check } from 'k6';

export let options = {
    stages: [
        { duration: '30s', target: 100 },  // Ramp up to 100 VUs over 1 minute
        { duration: '3m', target: 100 },  // Stay at 100 VUs for 3 minutes
        { duration: '30s', target: 0 },   // Ramp down to 0 over 30 seconds
    ],
    thresholds: {
        http_req_duration: ['p(95)<1000'],  // 95% requests below 1s
    },
};

export function setup() {
    // 1) Register
    let registerRes = http.post(
        'https://localhost:7041/api/Authentication/register',
        JSON.stringify({ username: 'stressUser', email: 'stress@example.com', password: 'StressPass123!' }),
        { headers: { 'Content-Type': 'application/json' } }
    );

    // 2) Login
    let loginRes = http.post(
        'https://localhost:7041/api/Authentication/login',
        JSON.stringify({ username: 'stressUser', password: 'StressPass123!' }),
        { headers: { 'Content-Type': 'application/json' } }
    );

    let success = check(loginRes, { 'logged in': (r) => r.status === 200 });
    if (!success) {
        console.log('Login response:', loginRes.body);
        throw new Error(`Login failed. Got status ${loginRes.status}`);
    }

    let body = JSON.parse(loginRes.body);
    return { token: body.data.token };
}

export default function (data) {
    let token = data.token;
    let headers = { Authorization: `Bearer ${token}` };

    let res = http.get('https://localhost:7041/api/Scenes', { headers });
    check(res, {
        'status is 200': (r) => r.status === 200,
    });

    sleep(1);
}
