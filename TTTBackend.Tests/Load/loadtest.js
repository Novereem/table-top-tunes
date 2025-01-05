import http from 'k6/http';
import { sleep, check } from 'k6';

// -- This is a "load test": moderate ramp, stay for a while, ramp down.
export let options = {
    stages: [
        { duration: '10s', target: 30 },  // Ramp up to 30 VUs over 10s
        { duration: '20s', target: 30 },  // Stay at 30 VUs for 20s
        { duration: '10s', target: 0 },   // Ramp down to 0 over 10s
    ],
};

// Reusable setup (register + login)
export function setup() {
    // 1) Register
    let registerRes = http.post(
        'https://localhost:7041/api/Authentication/register',
        JSON.stringify({ username: 'testuser', email: 'test@example.com', password: 'TestPassword123!' }),
        { headers: { 'Content-Type': 'application/json' } }
    );

    // 2) Login
    let loginRes = http.post(
        'https://localhost:7041/api/Authentication/login',
        JSON.stringify({ username: 'testuser', password: 'TestPassword123!' }),
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

    // Simulate user "think time"
    sleep(1);
}