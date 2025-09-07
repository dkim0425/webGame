from fastapi import FastAPI, WebSocket
from typing import List
import asyncio
import aioredis

app=FastAPI
clients=List[WebSocket]=[]

# Redis 설정
REDIS_URL="redis://localhost:6379"
redis=None

#서버가 시작될때 한번만 실행할 함수
@app.on_event("startup")
async def startup_event():
    global redis 
    redis= await aioredis.from_url(REDIS_URL,decode_responses=True)
    asyncio.create_task(redis_subscribe())
    


# WebSocket 연결 엔드포인트
@app.websocket("/ws")
async def websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    clients.append(websocket)
    try:
        while True:
            data = await websocket.receive_text()
            # 받은 메시지를 Redis 채널로 발행
            await redis.publish("game_channel", data)
    except Exception:
        clients.remove(websocket)

# Redis 구독 & 브로드캐스트
async def redis_subscribe():
    pubsub = redis.pubsub()
    await pubsub.subscribe("game_channel")
    async for message in pubsub.listen():
        if message['type'] == 'message':
            for client in clients:
                try:
                    await client.send_text(message['data'])
                except:
                    clients.remove(client)
