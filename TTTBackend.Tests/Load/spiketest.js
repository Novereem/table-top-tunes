import http from 'k6/http';
import { sleep, check } from 'k6';

export let options = {
    stages: [
        { duration: '5s', target: 100 },   // Sudden jump to 100 VUs in 5s
        { duration: '10s', target: 100 },  // Hold at 100 VUs for 10s
        { duration: '5s', target: 0 },     // Drop back down to 0 over 5s
    ],
};

export function setup() {
    // 1) Register
    let registerRes = http.post(
        'https://localhost:7041/api/Authentication/register',
        JSON.stringify({ username: 'spikeUser', email: 'spike@example.com', password: 'SpikePass123!' }),
        { headers: { 'Content-Type': 'application/json' } }
    );

    // 2) Login
    let loginRes = http.post(
        'https://localhost:7041/api/Authentication/login',
        JSON.stringify({ username: 'spikeUser', password: 'SpikePass123!' }),
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
